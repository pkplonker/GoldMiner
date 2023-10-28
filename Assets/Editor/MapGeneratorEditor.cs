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
				if (((mapGenerator.spawnedProps)) && mapGenerator.MapGeneratorTerrain.Generated)
				{
					if (GUILayout.Button("Regenerate Map"))
					{
						Respawn(mapGenerator);
					}
				}
				if (((mapGenerator.spawnedProps)) && mapGenerator.MapGeneratorTerrain.Generated)
				{
					if (GUILayout.Button("Regenerate Random Map"))
					{
						RespawnRandom(mapGenerator);
					}
				}
			}

			base.OnInspectorGUI();
		}
		[CheatCommand]
		private static void RespawnRandom(MapGenerator mapGenerator)
		{
			mapGenerator.RegenerateWorld(true);
		}
		[CheatCommand]
		private static void Respawn(MapGenerator mapGenerator)
		{
			mapGenerator.RegenerateWorld();
		}
	}
}