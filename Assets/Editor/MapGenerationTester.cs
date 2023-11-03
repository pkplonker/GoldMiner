//
// Copyright (C) 2023 Stuart Heath. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Player;
using Targets;
using TerrainGeneration;
using UnityEngine;
using UnityEditor;
using UnityEngine.PlayerLoop;

/// <summary>
///MapGenerationTester full description
/// </summary>
public class MapGenerationTester : EditorWindow
{
	private static MapGenerator mapGenerator;
	private static MapData mapdata;
	private static PropCollection propData;
	private static int startSeed;
	private static int endSeed;
	private static List<IMapGenerationTestResult> mapGenerationTestResults = new();
	private static int count = -1;
	private static bool stop;
	private static bool inProcess;

	[MenuItem("GoldMiner/Map Generation Tester")]
	public static void ShowWindow()
	{
		var window = GetWindow<MapGenerationTester>();
		window.titleContent = new GUIContent("Map Generation Tester");
	}

	private void OnGUI()
	{
		mapGenerator = ServiceLocator.Instance.GetService<MapGenerator>();

		EditorGUILayout.LabelField(new GUIContent($"Time Taken: {(TimeTaken() > 0 ? TimeTaken().ToString() : "0")}"));
		EditorGUILayout.LabelField(new GUIContent($"Seeds processed: {count}"));

		EditorGUILayout.Separator();
		EditorGUILayout.Separator();

		EditorGUILayout.BeginVertical();
		mapdata = (MapData) EditorGUILayout.ObjectField(new GUIContent("Mapdata"), mapdata, typeof(MapData), false);
		propData = (PropCollection) EditorGUILayout.ObjectField(new GUIContent("Prop collection"), propData,
			typeof(PropCollection),
			false);
		startSeed = EditorGUILayout.IntField(new GUIContent("Start Seed"), startSeed);
		endSeed = EditorGUILayout.IntField(new GUIContent("End Seed"), endSeed);
		EditorGUILayout.EndVertical();

		if (!Application.isPlaying) return;
		EditorGUILayout.Separator();
		if (mapdata != null && propData != null)
		{
			if (GUILayout.Button("Perform Test"))
			{
				PerformTest();
			}

			if (inProcess)
			{
				if (GUILayout.Button("Stop"))
				{
					stop = true;
				}
			}
		}

		if (mapGenerationTestResults.Count > 0)
		{
			EditorGUILayout.Separator();
			GUILayout.Label("Test Results", EditorStyles.boldLabel);
			GUILayout.Label($"Average Time Taken: {GetAverageTimeTaken()} ms");

			GUILayout.BeginVertical("box");
			GUILayout.Label("Failed Seeds:", EditorStyles.boldLabel);
			foreach (var result in mapGenerationTestResults.OfType<MapGenerationTestResultFail>())
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label($"Seed: {result.seed}, Reason: {result.reason}, Time Taken: {result.msTaken} ms");

				GUILayout.EndHorizontal();
			}

			GUILayout.EndVertical();
		}
	}

	private void OnEnable() => EditorApplication.update += UpdateAlways;

	private void OnDisable() => EditorApplication.update -= UpdateAlways;

	private void UpdateAlways() => Repaint();

	private static float GetAverageTimeTaken()
	{
		if (mapGenerationTestResults.Count == 0) return 0;
		return mapGenerationTestResults.Average(result => result.msTaken);
	}

	public static float TimeTaken() => mapGenerationTestResults.Sum(x => x.msTaken);

	public static void PerformTest()
	{
		inProcess = true;
		count = -1;
		mapGenerationTestResults.Clear();
		mapGenerator.MapGeneratedTime -= MapGeneratorOnMapGenerated;
		mapGenerator.MapGeneratedTime += MapGeneratorOnMapGenerated;
		mapGenerator.MapGeneratorTerrain.MapData = mapdata;
		mapGenerator.PropSpawner.PropCollections = propData;
		PerformNext();
	}

	private static void MapGeneratorOnMapGenerated(float mapTime, float propsTime)
	{
		var player = FindObjectOfType(typeof(PlayerMovement));
		var truck = FindObjectOfType(typeof(Truck));
		var result = player && truck;
		var timeTaken = mapTime + propsTime;
		if (result)
		{
			mapGenerationTestResults.Add(new MapGenerationTestResultPass()
				{msTaken = timeTaken, seed = mapGenerator.MapGeneratorTerrain.MapData.seed});
		}
		else
		{
			var data = new MapGenerationTestResultFail()
				{msTaken = timeTaken, seed = mapGenerator.MapGeneratorTerrain.MapData.seed};
			if (!player && !truck) data.reason = MapGenerationTestResultReason.All;
			if (!player) data.reason = MapGenerationTestResultReason.Player;
			if (!truck) data.reason = MapGenerationTestResultReason.Truck;

			mapGenerationTestResults.Add(data);
		}

		Thread.Sleep(500);
		PerformNext();
	}

	private static void PerformNext()
	{
		if (stop)
		{
			inProcess = false;
			stop = false;
			return;
		}

		count++;
		if (count + startSeed >= endSeed)
		{
			inProcess = false;
			return;
		}

		mapGenerator.RegenerateWorld(startSeed + count);
	}

	private struct MapGenerationTestResultPass : IMapGenerationTestResult
	{
		public float msTaken { get; set; }
		public int seed { get; set; }
	}

	private struct MapGenerationTestResultFail : IMapGenerationTestResult
	{
		public float msTaken { get; set; }
		public int seed { get; set; }
		public MapGenerationTestResultReason reason { get; set; }
	}

	public enum MapGenerationTestResultReason
	{
		All,
		Player,
		Truck,
	}

	private interface IMapGenerationTestResult
	{
		public float msTaken { get; set; }
		public int seed { get; set; }
	}
}