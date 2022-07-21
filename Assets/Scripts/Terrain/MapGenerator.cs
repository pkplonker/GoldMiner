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

		[Header("Core")] [Range(0, 80)] [SerializeField]
		private int size;

		[field: SerializeField] public bool autoUpdate { get; private set; }

		[Header("Noise")] [Range(0, 8f)] [SerializeField]
		private int octaves;

		[Range(0, 100)] [SerializeField] private float noiseScale;
		[Range(0, 1f)] [SerializeField] private float persistance;
		[Range(1, 10f)] [SerializeField] private float lacunarity;
		[SerializeField] private Vector2 offset;
		[SerializeField] private int seed;
		[SerializeField] private TerrainType[] regions;

		[Header("Output")] [SerializeField] private MapDisplay mapDisplay;
		[Header("Color")] [SerializeField] private DrawMode drawMode = DrawMode.Color;

		[Header("Mesh")] [Range(1, 5)] [SerializeField]
		private int vertexCountMultiplier;

		[Range(1f, 100f)] [SerializeField] private float heightMultiplier;
		[SerializeField] private AnimationCurve heightCurve;
		private void Awake() => mapDisplay = FindObjectOfType<MapDisplay>();


		public void GenerateMap()
		{
			var multipliedWidth = size * vertexCountMultiplier;
			var multipliedHeight = size * vertexCountMultiplier;
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
			else if (drawMode == DrawMode.Mesh)
			{
				mapDisplay.DrawMesh(
					TerrainMeshGenerator.GenerateTerrainMesh(noiseMap, heightMultiplier, heightCurve,
						vertexCountMultiplier),
					TextureGenerator.TextureFromColourMap(colourMap, multipliedWidth, multipliedHeight));
			}
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