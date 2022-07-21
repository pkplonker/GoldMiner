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

		[Header("Core")] [Range(0, 241)] [SerializeField] private int width;
		[Range(0, 241)][SerializeField] private int height;
		[field: SerializeField] public bool autoUpdate { get; private set; }

		[Header("Noise")]
		[Range(0, 8f)] [SerializeField]
		private int octaves;

		[Range(0, 100)] [SerializeField] private float noiseScale;
		[Range(0, 1f)] [SerializeField] private float persistance;
		[Range(1, 10f)] [SerializeField] private float lacunarity;
		[SerializeField] private Vector2 offset;
		[Range(1, 10f)] [SerializeField] private int seed;
		[SerializeField] private TerrainType[] regions;

		[Header("Output")]
		[SerializeField] private MapDisplay mapDisplay;
		[Header("Color")]
		[SerializeField] private DrawMode drawMode = DrawMode.Color;

		[Header("Mesh")]
		[Range(0,10)] [SerializeField] private int vertexCountMultiplier;
		[Range(1f, 100f)] [SerializeField] private float heightMultiplier;
		[SerializeField] private AnimationCurve heightCurve;
		private void Awake() => mapDisplay = FindObjectOfType<MapDisplay>();

		

		public void GenerateMap() {
			float[,] noiseMap = Noise.GenerateNoiseMap (width, height, seed, noiseScale, octaves, persistance, lacunarity, offset);

			Color[] colourMap = new Color[width * height];
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					float currentHeight = noiseMap [x, y];
					for (int i = 0; i < regions.Length; i++) {
						if (currentHeight <= regions [i].height) {
							colourMap [y * width + x] = regions [i].color;
							break;
						}
					}
				}
			}

			if (drawMode == DrawMode.Noise) {
				mapDisplay.DrawTexture (TextureGenerator.TextureFromHeightMap (noiseMap));
			} else if (drawMode == DrawMode.Color) {
				mapDisplay.DrawTexture (TextureGenerator.TextureFromColourMap (colourMap, width, height));
			} else if (drawMode == DrawMode.Mesh) {
				mapDisplay.DrawMesh (TerrainMeshGenerator.GenerateTerrainMesh (noiseMap,heightMultiplier,heightCurve,vertexCountMultiplier), TextureGenerator.TextureFromColourMap (colourMap, width, height));
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