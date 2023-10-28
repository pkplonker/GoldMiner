using System;
using UnityEngine;

namespace TerrainGeneration
{
	[CreateAssetMenu(fileName = "New Map Data", menuName = "Map Data")]
	[Serializable]
	public class MapData : ScriptableObject
	{
		[field: Header("Ranges")]
		[field: SerializeField] public RangeFloat PersistanceRange = new RangeFloat {Min = 0f, Max = 2f};
		[field: SerializeField] public RangeFloat LacunarityRange = new RangeFloat {Min = 0f, Max = 1f};
		[field: SerializeField] public RangeFloat NoiseScaleRange = new RangeFloat {Min = 1f, Max = 1000f};

		[field: SerializeField] public RangeFloat HeightMultiplierRange = new RangeFloat {Min = 0f, Max = 100f};
		
		[field: Header("Determined props")]
		

		[field: SerializeField] public float Persistance { get; private set; }
		[field: SerializeField] public float Lacunarity { get; private set; }
		[field: SerializeField] public float NoiseScale { get; private set; }
		[field: SerializeField] public float HeightMultiplier { get; private set; } = 5;
		
		[field: Header("Standard")]
		[field: SerializeField] public int ChunksPerRow { get; private set; }
		[field: SerializeField]
		public int MapChunkSize { get; private set; }
		[field: SerializeField] public int seed;
		[field: SerializeField] public Vector2 offset;
		[field: SerializeField] public Material material;
		[field: SerializeField] public AnimationCurve HeightCurve;
		[field: SerializeField] public string GroundLayer = "Ground";
		[field: SerializeField] public float BoundaryInstep = 50f;

		[field: Range(1, 8), SerializeField] public int Octaves;
		[Range(1, 10), SerializeField] public int LOD;

		public void InitialiseValues()
		{
			var random = new System.Random(seed);
			Persistance = (float) random.NextDouble() * (PersistanceRange.Max - PersistanceRange.Min) +
			              PersistanceRange.Min;
			Lacunarity = (float) random.NextDouble() * (LacunarityRange.Max - LacunarityRange.Min) +
			             LacunarityRange.Min;
			NoiseScale = (float) random.NextDouble() * (NoiseScaleRange.Max - NoiseScaleRange.Min) +
			             NoiseScaleRange.Min;
			// HeightMultiplier = (float) random.NextDouble() * (HeightMultiplierRange.Max - HeightMultiplierRange.Min) +
			//                    HeightMultiplierRange.Min;
		}

		public int GetSize() => MapChunkSize * ChunksPerRow;
	}

	[Serializable]
	public struct RangeInt
	{
		public int Min;
		public int Max;
	}

	[Serializable]
	public struct RangeFloat
	{
		public float Min;
		public float Max;
	}
}