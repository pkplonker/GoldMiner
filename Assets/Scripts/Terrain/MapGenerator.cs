using System;
using UnityEngine;

namespace Terrain
{
	public class MapGenerator : MonoBehaviour
	{
		[Range(0,241)] [SerializeField] private int size;
		[Range(0,100)][SerializeField] private float noiseScale;
		[SerializeField] private MapDisplay mapDisplay;
		[Range(0,2f)] [SerializeField] private int octaves;

		[Range(0,1f)] [SerializeField] private float persistance;
		[Range(1,10f)] [SerializeField] private float lacunarity;
		[Range(1,10f)] [SerializeField] private int seed;

		[SerializeField] private Vector2 offset;

		[field: SerializeField] public bool autoUpdate { get; private set; }

		private void Awake()
		{
			mapDisplay = FindObjectOfType<MapDisplay>();
		}

		public void GenerateMap()
		{
			float[,] noiseMap = Noise.GenerateNoiseMap(size, seed, noiseScale, octaves, persistance, lacunarity,offset);
			mapDisplay.DrawNoiseMap(noiseMap);
		}
	}
}