using System;
using UnityEngine;

namespace TerrainGeneration
{
	[Serializable]
	public struct MapData
	{
		[field: Range(1, 100), SerializeField]
		public int MapChunkSize { get; private set; } 

		[field: Range(1, 50), SerializeField]
		public int ChunksPerRow { get; private set; }

		[Range(1, 10), SerializeField] public int _lod;
		[field: SerializeField] public int _seed;

		[field: Range(1, 8), SerializeField] public int _octaves ;

		[field: Range(0f, 2f), SerializeField]
		public float _persistance ;

		[field: Range(0f, 1f), SerializeField] public float _lacunarity ;

		[field: SerializeField] public Vector2 _offset ;
		[field: SerializeField] public Material _material;

		[field: Range(0f, 1000f), SerializeField]
		public float _noiseScale ;

		[field: SerializeField] public Terrains[] _terrains;

		[field: Range(0f, 100f), SerializeField]
		public float _heightMultiplier;

		[field: SerializeField] public AnimationCurve _heightCurve;
		[field: SerializeField] public string _terrainLayer;
		public int GetSize() => MapChunkSize * ChunksPerRow;
	}
}