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

	[MenuItem("GoldMiner/Procgen MapViewer")]
	public static void MapViweer()
	{
		if (!Application.isPlaying)
		{
			Debug.Log("Can't open window in edit mode");
			return;
		}
		GetWindow<MapViewer>();
	}

	private void OnGUI()
	{
		float size = 650f;
		float padding = 10f;
		int mapsPerRow = Mathf.Max(1, Mathf.FloorToInt((position.width - padding) / (size + padding)));
		if (!Application.isPlaying) Close();
		var maps = MapStore.Instance.GetAllMaps();
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
