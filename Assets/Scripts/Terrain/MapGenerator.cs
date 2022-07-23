using System;
using UnityEngine;

namespace Terrain
{
	public class MapGenerator : MonoBehaviour
	{
		public enum DrawMode
		{
			Noise,
			Color,
			Mesh
		}

		[SerializeField] private Vector2 offset;
		[field: Header("Core")]
		[field: SerializeField]
		public int totalSize { get; private set; }

		[field: Range(0, 51)]
		[field: SerializeField]
		public int chunkSizeInVerts { get; private set; }

		[SerializeField] private GameObject terrainChunkPrefab;

		[Header("Noise")] [Range(0, 8f)] [SerializeField]
		private int octaves;

		[Range(0, 100)] [SerializeField] private float noiseScale;
		[Range(0, 1f)] [SerializeField] private float persistance;
		[Range(1, 10f)] [SerializeField] private float lacunarity;
		[SerializeField] private int seed;
		[SerializeField] private TerrainType[] regions;

		[Header("Output")] [SerializeField] private MapDisplay mapDisplay;
		[Header("Color")] [SerializeField] private DrawMode drawMode = DrawMode.Color;

		[Header("Mesh")] [Range(1, 5)] [SerializeField]
		private int vertexCountMultiplier;

		[Range(1f, 100f)] [SerializeField] private float heightMultiplier;
		[SerializeField] private AnimationCurve heightCurve;
		private void Awake() => mapDisplay = FindObjectOfType<MapDisplay>();

		private void Start()
		{
			GenerateMap();
		}

		public void GenerateMap()
		{
			var chunkSize = (chunkSizeInVerts - 1);
			var chunksPerRow = Mathf.CeilToInt(totalSize / (float) chunkSize);
			var startPos = -(chunksPerRow / 2) * chunkSize;

			for (var x = 0; x < chunksPerRow; x++)
			{
				for (var y = 0; y < chunksPerRow; y++)
				{
					var go = Instantiate(terrainChunkPrefab, transform, true);
					go.name = $"Chunk({x}:{y})";
					go.transform.localScale = Vector3.one;
					go.transform.localRotation = Quaternion.identity;
					go.transform.localPosition = new Vector3(startPos + (chunkSize * x), 0,
						startPos + (chunkSize * y));
					GenerateMapChunk(go.GetComponent<TerrainChunk>(), new Vector2(go.transform.position.x,go.transform.position.z));
				}
			}
		}

		private void Update()
		{
			GenerateTex();
		}

		public void GenerateTex()
		{
			var multipliedWidth = chunkSizeInVerts * vertexCountMultiplier;
			var multipliedHeight = chunkSizeInVerts * vertexCountMultiplier;
			float[,] noiseMap = Noise.GenerateNoiseMap(multipliedWidth, multipliedHeight, seed, noiseScale, octaves,
				persistance, lacunarity, offset, vertexCountMultiplier);

			Color[] colourMap = new Color[multipliedWidth * multipliedHeight];
			for (int y = 0; y < multipliedHeight; y++)
			{
				for (int x = 0; x < multipliedWidth; x++)
				{
					float currentHeight = noiseMap[x, y];
					for (int i = 0; i < regions.Length; i++)
					{
						if (currentHeight <= regions[i].height)
						{
							colourMap[y * multipliedWidth + x] = regions[i].color;
							break;
						}
					}
				}
			}

			if (drawMode == DrawMode.Noise)
			{
				mapDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
			}
			else if (drawMode == DrawMode.Color)
			{
				mapDisplay.DrawTexture(
					TextureGenerator.TextureFromColourMap(colourMap, multipliedWidth, multipliedHeight));
			}
		}

		public void GenerateMapChunk(TerrainChunk terrainChunk, Vector2 offset)
		{
			var multipliedWidth = chunkSizeInVerts * vertexCountMultiplier;
			var multipliedHeight = chunkSizeInVerts * vertexCountMultiplier;
			float[,] noiseMap = Noise.GenerateNoiseMap(multipliedWidth, multipliedHeight, seed, noiseScale, octaves,
				persistance, lacunarity, offset, vertexCountMultiplier);

			Color[] colourMap = new Color[multipliedWidth * multipliedHeight];
			for (int y = 0; y < multipliedHeight; y++)
			{
				for (int x = 0; x < multipliedWidth; x++)
				{
					float currentHeight = noiseMap[x, y];
					for (int i = 0; i < regions.Length; i++)
					{
						if (currentHeight <= regions[i].height)
						{
							colourMap[y * multipliedWidth + x] = regions[i].color;
							break;
						}
					}
				}
			}


			terrainChunk.DrawMesh(
				TerrainMeshGenerator.GenerateTerrainMesh(noiseMap, heightMultiplier, heightCurve,
					vertexCountMultiplier),
				TextureGenerator.TextureFromColourMap(colourMap, multipliedWidth, multipliedHeight));
		}
	}


	[System.Serializable]
	public struct TerrainType
	{
		public string name;
		public float height;
		public Color color;
	}
}