using System;
using UnityEngine;

namespace TerrainGeneration
{
	[Serializable]
	public struct MapData
	{
		[field: Range(1, 100), SerializeField]
		public int mapChunkSize { get; private set; } 

		[field: Range(1, 50), SerializeField]
		public int chunksPerRow { get; private set; }

		[Range(1, 10), SerializeField] public int LOD;
		[field: SerializeField] public int seed;

		[field: Range(1, 8), SerializeField] public int octaves ;

		[field: Range(0f, 2f), SerializeField]
		public float persistance ;

		[field: Range(0f, 1f), SerializeField] public float lacunarity ;

		[field: SerializeField] public Vector2 offset ;
		[field: SerializeField] public Material material;

		[field: Range(0f, 1000f), SerializeField]
		public float noiseScale ;

		[field: SerializeField] public Terrains[] terrains;

		[field: Range(0f, 100f), SerializeField]
		public float heightMultiplier;

		[field: SerializeField] public AnimationCurve heightCurve;
		[field: SerializeField] public String terrainLayer;
		public int GetSize() => mapChunkSize * chunksPerRow;
	}
}