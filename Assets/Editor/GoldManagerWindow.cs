//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using Targets;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using Object = UnityEngine.Object;

/// <summary>
///GoldManager full description
/// </summary>
public class GoldManagerWindow : EditorWindow
{
	private int columnWidth = 400;
	private Vector2 scrollPos;
	private List<Gold> currentDisplayedItems;
	private static GoldSpawnManager goldSpawnManager;

	[MenuItem("GoldMiner/Gold Status Window")]
	public static void GoldStatusWindow()
	{
		if (!Application.isPlaying) return;
		GetWindow<GoldManagerWindow>();
		goldSpawnManager = GoldSpawnManager.Instance;
	}

	private void OnGUI()
	{
		if (!Application.isPlaying) Close();
		EditorGUILayout.LabelField("Column width");
		columnWidth = EditorGUILayout.IntField(columnWidth);
		DrawButtons();
		DrawHeaders();
		DrawTotals();
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
		if (currentDisplayedItems != null)
		{
			foreach (var item in currentDisplayedItems)
			{
				DrawRow(item);
			}
		}

		EditorGUILayout.EndScrollView();
		Repaint();
	}

	private void DrawButtons()
	{
		GUILayout.BeginHorizontal();
		if (goldSpawnManager == null)
		{
			EditorGUILayout.LabelField("Gold Manager not yet found", EditorStyles.boldLabel,
				GUILayout.Width(columnWidth));
		}
		else
		{
			if (GUILayout.Button("ShowAll"))
				currentDisplayedItems = goldSpawnManager._goldPiecesSpawned.OrderByDescending(g => g.Weight).ToList();
			if (GUILayout.Button("Show Collected"))
				currentDisplayedItems = goldSpawnManager._goldPiecesFound.OrderByDescending(g => g.Weight).ToList();
			if (GUILayout.Button("Show Uncollected"))
				currentDisplayedItems =
					goldSpawnManager._goldPiecesSpawned.Except(goldSpawnManager._goldPiecesFound)
						.OrderByDescending(g => g.Weight).ToList();
		}

		GUILayout.EndHorizontal();
	}

	private void DrawTotals()
	{
		GUILayout.BeginHorizontal();
		var c = goldSpawnManager._goldPiecesSpawned.Sum(g => g.Weight);

		EditorGUILayout.LabelField("Total Gold on Map = " + c + "[ £" + c * GoldPrice.goldPrice + "]",
			EditorStyles.boldLabel, GUILayout.Width(columnWidth));
		c = goldSpawnManager._goldPiecesFound.Sum(g => g.Weight);

		EditorGUILayout.LabelField("Total Gold found =" + c + "[ £" + c * GoldPrice.goldPrice + "]",
			EditorStyles.boldLabel, GUILayout.Width(columnWidth));
		c = goldSpawnManager._goldPiecesSpawned.Except(goldSpawnManager._goldPiecesFound).Sum(g => g.Weight);
		EditorGUILayout.LabelField("Total Gold remaining =" + c + "[ £" + c * GoldPrice.goldPrice + "]",
			EditorStyles.boldLabel, GUILayout.Width(columnWidth));

		GUILayout.EndHorizontal();
	}

	private void DrawHeaders()
	{
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Name", EditorStyles.boldLabel, GUILayout.Width(columnWidth));
		EditorGUILayout.LabelField("Location", EditorStyles.boldLabel, GUILayout.Width(columnWidth));
		EditorGUILayout.LabelField("Weight", EditorStyles.boldLabel, GUILayout.Width(columnWidth));

		GUILayout.EndHorizontal();
	}

	private void DrawRow(Gold gold)
	{
		if (gold == null) return;
		GUILayout.BeginHorizontal();
		if (EditorGUILayout.LinkButton(gold.name, GUILayout.Width(columnWidth)))
		{
			Selection.objects = new Object[] {gold.gameObject};
			Selection.activeTransform = gold.transform;
			SceneView.lastActiveSceneView.FrameSelected();
		}

		EditorGUILayout.LabelField(gold.transform.position.ToString(), EditorStyles.boldLabel,
			GUILayout.Width(columnWidth));
		EditorGUILayout.LabelField(gold.Weight.ToString(), EditorStyles.boldLabel, GUILayout.Width(columnWidth));
		GUILayout.EndHorizontal();
	}
}