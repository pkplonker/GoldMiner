//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using TerrainGeneration;
using UnityEditor;
using UnityEngine;

namespace Editor
{
	/// <summary>
	///MapGenerator full description
	/// </summary>
	[CustomEditor(typeof(MapGenerator))]
	public class MapGeneratorEditor : UnityEditor.Editor
	{
		/*public override void OnInspectorGUI()
		{
			var mapGenerator = (MapGenerator) target;
			if (Application.isPlaying)
			{
				if (((mapGenerator.spawnedProps)) && mapGenerator.MapGeneratorTerrain.Generated)
				{
					if (GUILayout.Button("Regenerate Map"))
					{
						if (mapGenerator.PropSpawner.Container != null)
						{
							foreach (var v in mapGenerator.PropSpawner.Container.GetComponentsInChildren<Transform>())
							{
								if (v == mapGenerator.transform || v == mapGenerator.PropSpawner.Container) continue;
								Destroy(v.gameObject);
							}
						}

						mapGenerator.MapGeneratorTerrain.ClearData();
						mapGenerator.SpawnTerrain();
						mapGenerator.spawnedProps = false;
					}
				}
			}

			base.OnInspectorGUI();
		}*/
	}
}