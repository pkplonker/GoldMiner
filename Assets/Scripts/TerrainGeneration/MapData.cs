using System;
using UnityEngine;

namespace TerrainGeneration
{
	[CreateAssetMenu(fileName = "New Map Data", menuName = "Map Data")]
	[Serializable]
	public class MapData : ScriptableObject
	{
		[field: Range(1, 100), SerializeField]
		public int MapChunkSize { get; private set; }

		[field: Range(1, 50), SerializeField]
		public int ChunksPerRow { get; private set; }

		// [field: SerializeField]public int BorderChangeDistance { get; private set; }
		// [field: SerializeField]public int BorderSize { get; private set; }
		// [field: SerializeField]public int BorderDistance { get; private set; }

		[Range(1, 10), SerializeField] public int lod;
		[field: SerializeField] public int seed;

		[field: Range(1, 8), SerializeField] public int octaves ;

		[field: Range(0f, 2f), SerializeField]
		public float Persistance = 0.8f ;

		[field: Range(0f, 1f), SerializeField] public float lacunarity =0.8f ;

		[field: SerializeField] public Vector2 offset ;
		[field: SerializeField] public Material material;

		[field: Range(0f, 1000f), SerializeField]
		public float NoiseScale =16 ;

		[field: SerializeField] public Terrains[] terrains;

		[field: Range(0f, 100f), SerializeField]
		public float heightMultiplier;

		[field: SerializeField] public AnimationCurve heightCurve;
		public int GetSize() => MapChunkSize * ChunksPerRow;
		[field: SerializeField] public string groundLayer { get; private set; } = "Ground";
		[field: SerializeField] public float boundryInstep { get; private set; } = 50f;

	}
}