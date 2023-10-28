using TerrainGeneration;
using UnityEngine;

namespace Props
{
	[CreateAssetMenu(fileName = "Surface Prop", menuName = "Props/Surface Prop")]
	public class SurfaceProp : Prop
	{
		[field: Range(-1f, 1f), SerializeField]
		public float DropIntoTerrainAmount { get; protected set; }

		[field: Range(0, 1f), SerializeField] public float MinHeightNormalised { get; protected set; } = 0f;
		[field: Range(0, 1f), SerializeField] public float MaxHeightNormalised { get; protected set; } =0.75f;

		protected override Vector3 CalculatePosition(Vector3 position, MapData mapData, float factor = 10)
		{
			position = base.CalculatePosition(position, mapData, factor);
			var normalisedHeight = position.y / mapData.HeightMultiplier;
			return (normalisedHeight > MinHeightNormalised && normalisedHeight < MaxHeightNormalised)
				? position
				: Vector3.positiveInfinity;
		}
	}
}