using TerrainGeneration;
using UnityEngine;

namespace Props
{
	[CreateAssetMenu(fileName = "Surface Prop", menuName = "Props/Surface Prop")]
	public class SurfaceProp : Prop
	{
		[field: Range(-1f, 1f), SerializeField]
		public float DropIntoTerrainAmount { get; protected set; }

		protected override Vector3
			CalculatePosition(Vector3 position, MarchingCubeMapData mapData, float factor = 10) =>
			base.CalculatePosition(position, mapData, factor) - new Vector3(0, -DropIntoTerrainAmount, 0);
	}
}