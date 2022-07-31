using TerrainGeneration;
using UnityEditor;
using UnityEngine;

namespace Editor
{
	[CustomEditor(typeof(MapGeneratorTerrain))]
	public class MapGeneratorTerrainEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			MapGeneratorTerrain mapGeneratorTerrain = (MapGeneratorTerrain) target;


			/*if (GUILayout.Button("Generate"))
			{
				Generate(mapGeneratorTerrain);
			}

*/
			DrawDefaultInspector();
		}

		private static void Generate(MapGeneratorTerrain mapGeneratorTerrain)
		{
			if (Application.isPlaying)
			{
				if (mapGeneratorTerrain.container != null)
				{
					foreach (var v in mapGeneratorTerrain.container.GetComponentsInChildren<Transform>())
					{
						if (v == mapGeneratorTerrain.transform) continue;
						Destroy(v.gameObject);
					}
				}

				mapGeneratorTerrain.generated = false;
				mapGeneratorTerrain.Generate();
			}
		}
	}
}