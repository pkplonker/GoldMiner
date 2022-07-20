 //
 // Copyright (C) 2022 Stuart Heath. All rights reserved.
 //

 using Terrain;
 using UnityEditor;
 using UnityEngine;

 namespace Editor
 {
	 /// <summary>
	 ///MapDisplayEditor full description
	 /// </summary>
	 [CustomEditor(typeof(MapGenerator))]
	 public class MapDisplayEditor : UnityEditor.Editor
	 {
		 public override void OnInspectorGUI()
		 {
			 MapGenerator mapGenerator = target as MapGenerator;
			 if (DrawDefaultInspector())
			 {
				 if (mapGenerator.autoUpdate)
				 {
					 mapGenerator.GenerateMap();
				 }
			 }
			 if (GUILayout.Button("Generate"))
			 {
				 mapGenerator.GenerateMap();
			 }
		 }
	 }
 }
