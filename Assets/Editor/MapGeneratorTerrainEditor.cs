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
				if (mapGeneratorTerrain.Container != null)
				{
					foreach (var v in mapGeneratorTerrain.Container.GetComponentsInChildren<Transform>())
					{
						if (v == mapGeneratorTerrain.transform) continue;
						Destroy(v.gameObject);
					}
				}

				mapGeneratorTerrain.Generated = false;
				mapGeneratorTerrain.Generate();
			}
		}
	}
}