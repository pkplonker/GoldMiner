 //
 // Copyright (C) 2022 Stuart Heath. All rights reserved.
 //

 using Terrain;
 using UnityEditor;
 using UnityEngine;

 namespace Editor
 {
	 /// <summary>
	 ///TileGenerationEditor full description
	 /// </summary>
	 [CustomEditor(typeof(LevelGeneration))]
	 public class LevelGenerationEditor : UnityEditor.Editor
	 {
		 public override void OnInspectorGUI()
		 {
			 LevelGeneration t = (LevelGeneration)target ;

			 base.OnInspectorGUI();
			 if(GUILayout.Button("Generate"))
			 {
				 Transform[] trans = t.GetComponentsInChildren<Transform>();
				 foreach (var tran in trans)
				 {
					 if (tran.transform == t.transform) continue;
					 Destroy(tran.gameObject);
				 }
				 t.GenerateMap();
			 }
		 }
	 }
 }
