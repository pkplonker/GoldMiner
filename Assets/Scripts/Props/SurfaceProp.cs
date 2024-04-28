using TerrainGeneration;
using UnityEngine;

namespace Props
{
	[CreateAssetMenu(fileName = "Surface Prop", menuName = "Props/Surface Prop")]
	public class SurfaceProp : Prop
	{
		[field: Range(-1f, 1f), SerializeField]
		public float DropIntoTerrainAmount { get; protected set; }
	}
}