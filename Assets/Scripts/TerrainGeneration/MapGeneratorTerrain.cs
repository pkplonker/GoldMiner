using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using UnityEngine;

namespace TerrainGeneration
{
	public class MapGeneratorTerrain : MonoBehaviour
	{
		// [SerializeField] private float exponent = 1f;
		// [SerializeField] private float noiseScale = 0.1f;
		// [SerializeField] private float noiseWeight = 0.1f;

		[field: SerializeField] public MapData MapData { get; set; }
		private readonly ConcurrentQueue<TerrainChunkData> terrainChunkDatasQueue = new();
		private int chunksGeneratedCount;
		[SerializeField] private TerrainChunk chunkPrefab;
		private float[,] noiseMap;
		private float[,] falloffMap;

		public Transform Container { get; private set; }
		public static event Action OnTerrainGenerated;
		public static event Action<int, int> OnChunkGenerated;

		public static TerrainChunk[,] terrainChunks;
		public static TerrainChunkData[,] terrainChunkData;
		private Coroutine awaitChunkCor;
		public static event Action<float[,],MapData> OnNoiseMapGenerated;

		private void Awake()
		{
			CheatConsole.Instance.RegisterCommand("RandomiseSeed",
				() => MapData.seed = UnityEngine.Random.Range(0, 64000));
		}

		public float[,] NoiseMap
		{
			get => noiseMap;
			private set => noiseMap = value;
		}

#if UNITY_EDITOR
		public bool Generated { get; set; } = true;
#else
		public bool Generated { get; private set; }
#endif

		public void ClearData()
		{
			terrainChunkDatasQueue.Clear();
			if (Container)
			{
				foreach (Transform t in Container.GetComponentInChildren<Transform>())
				{
					if (t == Container) continue;
					var tc = t.GetComponent<TerrainChunk>();
					
					if(tc!=null)Destroy(tc);
					var mr = t.GetComponent<MeshFilter>();
					if (mr!=null)
					{
						if (mr.sharedMesh != null)
						{
							Destroy(mr.sharedMesh);
						}
						Destroy(mr);
					}
					
					var mf = t.GetComponent<MeshRenderer>();
					if (mf!=null)
					{
						if (mf.material != null)
						{
							Destroy(mf.material);
						}
						Destroy(mf);
					}
					Destroy(t.gameObject);
				}
			}

			chunksGeneratedCount = 0;
			Generated = false;
			Resources.UnloadUnusedAssets();
			GC.Collect();
		}

		public void Generate()
		{
			ClearData();
			StopCor();
			MapData.InitialiseValues();
			var chunksRequired = MapData.ChunksPerRow * MapData.ChunksPerRow;
			terrainChunks = new TerrainChunk[MapData.ChunksPerRow, MapData.ChunksPerRow];
			terrainChunkData = new TerrainChunkData[MapData.ChunksPerRow, MapData.ChunksPerRow];
			if (!Container)
			{
				Container = new GameObject() {name = "Tile Container"}.transform;
				Container.SetParent(transform);
			}

			var vertsPerRow = (MapData.MapChunkSize * MapData.LOD) + 1;
			var mapSize = vertsPerRow * MapData.ChunksPerRow;
			NoiseMap = Noise.GenerateNoiseMap(mapSize, mapSize, MapData.seed, MapData.NoiseScale * MapData.LOD,
				MapData.Octaves,
				MapData.Persistance,
				MapData.Lacunarity,
				new Vector2(MapData.offset.x, MapData.offset.y));
			OnNoiseMapGenerated?.Invoke(NoiseMap,MapData);

			awaitChunkCor = StartCoroutine(AwaitChunkDataCor(chunksRequired));
			for (var x = 0; x < MapData.ChunksPerRow; x++)
			{
				for (var y = 0; y < MapData.ChunksPerRow; y++)
				{
					var x1 = x;
					var y1 = y;
					Task.Run(() => new TerrainChunkDataGenerator().Init(AddToTerrainChunkQueue, x1, y1,
						MapData, NoiseMap));
				}
			}
		}

		private void StopCor()
		{
			if (awaitChunkCor != null) StopCoroutine(awaitChunkCor);
		}

		private IEnumerator AwaitChunkDataCor(int chunksRequired)
		{
			var cachedTime = Time.realtimeSinceStartup;
			var allowedTimePerFrame = 1 / 100f;
			while (chunksGeneratedCount != chunksRequired)
			{
				if (Time.realtimeSinceStartup - cachedTime >= allowedTimePerFrame)
				{
					yield return null;
					cachedTime = Time.realtimeSinceStartup;
				}

				if (!terrainChunkDatasQueue.TryDequeue(out var terrainChunkData)) continue;
				MapGeneratorTerrain.terrainChunkData[terrainChunkData.X, terrainChunkData.Y] = terrainChunkData;
				GenerateGameObject(terrainChunkData);
				chunksGeneratedCount++;

				OnChunkGenerated?.Invoke(chunksGeneratedCount, chunksRequired);
			}

			chunksGeneratedCount = 0;
			Generated = true;
			OnTerrainGenerated?.Invoke();
			AlignNormals();
		}

		private void AlignNormals()
		{
			foreach (var chunk in terrainChunks)
			{
				if(chunk!=null) chunk.ProcessNormalAlignment();
			}
		}

		private void AddToTerrainChunkQueue(TerrainChunkData terrainChunkData)
		{
			lock (terrainChunkDatasQueue)
			{
				terrainChunkDatasQueue.Enqueue(terrainChunkData);
			}
		}

		private void GenerateGameObject(TerrainChunkData tcd)
		{
			var ter = Instantiate(chunkPrefab,
				new Vector3(tcd.X * MapData.MapChunkSize, 0, tcd.Y * MapData.MapChunkSize),
				Quaternion.identity, Container);
			ter.Generate(MapData, tcd);
			terrainChunks[tcd.X, tcd.Y] = ter;
		}

		public static Texture2D TextureFromColourMap(Color[] colourMap, int vertsPerRow)
		{
			var texture = new Texture2D(vertsPerRow, vertsPerRow)
			{
				filterMode = FilterMode.Bilinear,
				wrapMode = TextureWrapMode.Clamp
			};
			texture.SetPixels(colourMap);
			texture.Apply();
			return texture;
		}

		public static TerrainChunk GetChunkFromPosition(MapData MapData, Vector3 position)
		{
			var xCoord = (int) position.x / MapData.MapChunkSize;
			var zCoord = (int) position.z / MapData.MapChunkSize;
			xCoord = Mathf.Clamp(xCoord, 0, terrainChunks.Length - 1);
			zCoord = Mathf.Clamp(zCoord, 0, terrainChunks.Length - 1);
			var terrain = terrainChunks[xCoord, zCoord];
			return terrain;
		}

		public TerrainChunk GetChunkFromPosition(Vector3 position)
		{
			var xCoord = (int) position.x / MapData.MapChunkSize;
			var zCoord = (int) position.z / MapData.MapChunkSize;
			xCoord = Mathf.Clamp(xCoord, 0, terrainChunks.Length - 1);
			zCoord = Mathf.Clamp(zCoord, 0, terrainChunks.Length - 1);
			var terrain = terrainChunks[xCoord, zCoord];
			return terrain;
		}

		public Vector2Int GetChunkIndexFromPosition(Vector3 position)
		{
			var xCoord = (int) position.x / MapData.MapChunkSize;
			var zCoord = (int) position.z / MapData.MapChunkSize;
			xCoord = Mathf.Clamp(xCoord, 0, terrainChunks.Length - 1);
			zCoord = Mathf.Clamp(zCoord, 0, terrainChunks.Length - 1);
			return new Vector2Int(xCoord, zCoord);
		}
	}

	[Serializable]
	public struct Terrains
	{
		public Color _color;
		[Range(0, 1)] public float _height;
	}
}