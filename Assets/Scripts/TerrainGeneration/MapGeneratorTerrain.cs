using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;

namespace TerrainGeneration
{
	public class MapGeneratorTerrain : MonoBehaviour
	{
		[field: SerializeField] public MapData MapData { get; private set; }
		private readonly ConcurrentQueue<TerrainChunkData> _terrainChunkDatas = new ConcurrentQueue<TerrainChunkData>();
		private int _chunksGeneratedCount = 0;
		[SerializeField] private TerrainChunk _chunkPrefab;
		private float[,] _noiseMap;
		public Transform Container { get; private set; }
		public static event Action OnTerrainGenerated;
		public static event Action<int, int> OnChunkGenerated;

		public static TerrainChunk[,] terrainChunks;

		public float[,] NoiseMap
		{
			get => _noiseMap;
			private set => _noiseMap = value;
		}
#if UNITY_EDITOR
		public bool Generated { get; set; } = true;
#else
		public bool generated { get; private set; }
#endif

		public void ClearData()
		{
			_terrainChunkDatas.Clear();
			if (Container)
			{
				foreach (Transform t in Container.GetComponentInChildren<Transform>())
				{
					if (t == Container) continue;
					Destroy(t.gameObject);
				}
			}

			_chunksGeneratedCount = 0;
			Generated = false;
		}


		public void Generate()
		{
			ClearData();
			var chunksRequired = MapData.ChunksPerRow * MapData.ChunksPerRow;
			terrainChunks = new TerrainChunk[MapData.ChunksPerRow, MapData.ChunksPerRow];


			if (!Container)
			{
				Container = new GameObject() {name = "Tile Container"}.transform;
				Container.SetParent(transform);
			}

			var vertsPerRow = (MapData.MapChunkSize * MapData._lod) + 1;
			var mapSize = vertsPerRow * MapData.ChunksPerRow;
			NoiseMap = Noise.GenerateNoiseMap(mapSize, mapSize, MapData._seed, MapData._noiseScale * MapData._lod,
				MapData._octaves,
				MapData._persistance,
				MapData._lacunarity,
				new Vector2(MapData._offset.x, MapData._offset.y));
			StartCoroutine(AwaitChunkDataCor(chunksRequired));
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

		private IEnumerator AwaitChunkDataCor(int chunksRequired)
		{
			var cachedTime = Time.realtimeSinceStartup;
			var allowedTimePerFrame = 1 / 100f;
			while (_chunksGeneratedCount != chunksRequired)
			{
				if (Time.realtimeSinceStartup - cachedTime >= allowedTimePerFrame)
				{
					yield return null;
					cachedTime = Time.realtimeSinceStartup;
				}

				if (!_terrainChunkDatas.TryDequeue(out var terrainChunkData)) continue;
				GenerateGameObject(terrainChunkData);
				_chunksGeneratedCount++;

				OnChunkGenerated?.Invoke(_chunksGeneratedCount, chunksRequired);
			}

			_chunksGeneratedCount = 0;
			Generated = true;
			OnTerrainGenerated?.Invoke();
		}


		private void AddToTerrainChunkQueue(TerrainChunkData terrainChunkData)
		{
			lock (_terrainChunkDatas)
			{
				_terrainChunkDatas.Enqueue(terrainChunkData);
			}
		}


		private void GenerateGameObject(TerrainChunkData tcd)
		{
			var ter = Instantiate(_chunkPrefab,
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
	}

	[Serializable]
	public struct Terrains
	{
		public Color _color;
		[Range(0, 1)] public float _height;
	}
}