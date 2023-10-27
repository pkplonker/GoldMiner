 //
 // Copyright (C) 2023 Stuart Heath. All rights reserved.
 //
using UnityEngine;
using UnityEditor;

    /// <summary>
    ///MapViewer full description
    /// </summary>
public class MapViewer : EditorWindow
{
	Vector2 scrollPosition = Vector2.zero;
	private MapStore mapstore;
	[MenuItem("GoldMiner/Procgen MapViewer")]
	public static void MapViweer()=> GetWindow<MapViewer>();
	

	private void OnGUI()
	{
		if (!Application.isPlaying)
		{
			EditorGUILayout.LabelField("Waiting for game");
			return;
		}
		float size = 650f;
		float padding = 10f;
		int mapsPerRow = Mathf.Max(1, Mathf.FloorToInt((position.width - padding) / (size + padding)));
		if (!Application.isPlaying) Close();
		var maps = mapstore.allMaps;
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, true, false);

		for (var i = 0; i < maps.Count; i++)
		{
			int row = i / mapsPerRow;
			int col = i % mapsPerRow;
			float x = padding + (size + padding) * col;
			float y = padding + (size + padding) * row;

			GUI.DrawTexture(new Rect(x, y, size, size), maps[i].Item2);
			GUI.Label(new Rect(x, y + size, size, 20), maps[i].Item1);
		}

		GUILayout.EndScrollView();
	}

}
