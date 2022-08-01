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
		[field: SerializeField] public MapData mapData { get; private set; }
		private ConcurrentQueue<TerrainChunkData> terrainChunkDatas = new ConcurrentQueue<TerrainChunkData>();
		private int chunksGeneratedCount = 0;
		[SerializeField] private TerrainChunk chunkPrefab;
		public Transform container { get; private set; }
		public static event Action OnTerrainGenerated;
		public static event Action<int,int> OnChunkGenerated;

		public float[,] noiseMap { get; private set; }
#if UNITY_EDITOR
		public bool generated { get; set; } = true;
#else
		public bool generated { get; private set; }
#endif

		public void ClearData()
		{
			terrainChunkDatas.Clear();
			if (container)
			{
				foreach (Transform t in container.GetComponentInChildren<Transform>())
				{
					if (t == container) continue;
					Destroy(t.gameObject);
				}
			}

			chunksGeneratedCount = 0;
			generated = false;
		}


		public void Generate()
		{
			var chunksRequired = mapData.chunksPerRow * mapData.chunksPerRow;


			if (!container)
			{
				container = new GameObject() {name = "Tile Container"}.transform;
				container.SetParent(transform);
			}

			var vertsPerRow = (mapData.mapChunkSize * mapData.LOD) + 1;
			var mapSize = vertsPerRow * mapData.chunksPerRow;
			noiseMap = Noise.GenerateNoiseMap(mapSize, mapSize, mapData.seed, mapData.noiseScale*mapData.LOD,
				mapData.octaves,
				mapData.persistance,
				mapData.lacunarity,
				new Vector2(mapData.offset.x, mapData.offset.y));
			StartCoroutine(AwaitChunkDataCor(chunksRequired));
			for (var x = 0; x < mapData.chunksPerRow; x++)
			{
				for (var y = 0; y < mapData.chunksPerRow; y++)
				{
					var x1 = x;
					var y1 = y;
					Task.Run(() => new TerrainChunkDataGenerator().Init(AddToTerrainChunkQueue, x1, y1,
						mapData,noiseMap));
				}
			}
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

				if (!terrainChunkDatas.TryDequeue(out var terrainChunkData)) continue;
				GenerateGameObject(terrainChunkData);
				chunksGeneratedCount++;

				OnChunkGenerated?.Invoke(chunksGeneratedCount,chunksRequired);
			}

			chunksGeneratedCount = 0;
			generated = true;
			OnTerrainGenerated?.Invoke();
		}


		private void AddToTerrainChunkQueue(TerrainChunkData terrainChunkData)
		{
			lock (terrainChunkDatas)
			{
				terrainChunkDatas.Enqueue(terrainChunkData);
			}
		}


		private void GenerateGameObject(TerrainChunkData tcd)
		{
			Instantiate(chunkPrefab,
				new Vector3(tcd.x * mapData.mapChunkSize, 0, tcd.y * mapData.mapChunkSize),
				Quaternion.identity, container).Generate(mapData, tcd);
		}

		public static Texture2D TextureFromColourMap(Color[] colourMap, int vertsPerRow)
		{
			var texture = new Texture2D(vertsPerRow, vertsPerRow)
			{
				filterMode = FilterMode.Point,
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
		public Color color;
		[Range(0, 1)] public float height;
	}
}