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
		public override void OnInspectorGUI()
		{
			var mapGenerator = (MapGenerator) target;
			if (Application.isPlaying)
			{
				if (((mapGenerator.spawnedProps)) && mapGenerator.mapGeneratorTerrain.generated)
				{
					if (GUILayout.Button("Regenerate Map"))
					{
						if (mapGenerator.propSpawner.container != null)
						{
							foreach (var v in mapGenerator.propSpawner.container.GetComponentsInChildren<Transform>())
							{
								if (v == mapGenerator.transform || v == mapGenerator.propSpawner.container) continue;
								Destroy(v.gameObject);
							}
						}

						mapGenerator.mapGeneratorTerrain.ClearData();
						mapGenerator.SpawnTerrain();
						mapGenerator.spawnedProps = false;
					}
				}
			}

			base.OnInspectorGUI();
		}
	}
}