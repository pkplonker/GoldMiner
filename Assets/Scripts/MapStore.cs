using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using StuartHeathTools;
using TerrainGeneration;
using UnityEngine;

public class MapStore : GenericUnitySingleton<MapStore>
{
	private ObservableCollection<Tuple<string, Texture2D>> allMaps = new();

	private void Start()
	{
		allMaps.Clear();
		InGameMap.OnMapGenerated += OnColorMapGenerated;
		allMaps.CollectionChanged += MapCollectionChanged;
	}

	private void MapCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		foreach (var item in e.NewItems)
		{
			if (item is Tuple<string, Texture2D> tuple)
			{
				WriteToFile(tuple.Item2, tuple.Item1);
			}
		}
	}

	private static void WriteToFile(Texture2D combinedTexture, string name)
	{
		var pngData = combinedTexture.EncodeToPNG();
		if (pngData == null || pngData.Length == 0)
		{
			Debug.LogError("Failed to encode texture to PNG");
			return;
		}
		var savePath = Path.Combine(Application.persistentDataPath, name + ".png");
		File.WriteAllBytes(savePath, pngData);
		//Debug.Log("Combined texture saved as PNG: " + savePath);
	}

	private void OnDisable()
	{
		InGameMap.OnMapGenerated -= OnColorMapGenerated;
		MapGeneratorTerrain.OnNoiseMapGenerated -= OnNoiseMapGenerated;
	}

	private void OnColorMapGenerated(Texture2D obj) =>
		allMaps.Add(new Tuple<string, Texture2D>("Color", obj));

	private void OnFallOffMapGenerated(float[,] map) =>
		allMaps.Add(new Tuple<string, Texture2D>("Falloff", GenerateTexture(map)));

	private void OnNoiseMapGenerated(float[,] map) =>
		allMaps.Add(new Tuple<string, Texture2D>("Noise", GenerateTexture(map)));

	private void OnCombinedMapGenerated(float[,] map) =>
		allMaps.Add(new Tuple<string, Texture2D>("Combined", GenerateTexture(map)));

	public List<Tuple<string, Texture2D>> GetAllMaps() => allMaps.ToList();

	private Texture2D GenerateTexture(float[,] map)
	{
		if (map == null) return null;
		var colorMap = new Color[map.GetLength(0) * map.GetLength(1)];

		for (var y = 0; y < map.GetLength(1); y++)
		{
			for (var x = 0; x < map.GetLength(0); x++)
			{
				colorMap[y * map.GetLength(0) + x] = Color.Lerp(Color.black, Color.white, map[x, y]);
			}
		}

		return GenerateTextureFromColor(colorMap);
	}

	private Texture2D GenerateTextureFromColor(Color[] map)
	{
		var size = (int) Mathf.Sqrt(map.Length);
		var texture = new Texture2D(size, size);
		texture.SetPixels(map);
		texture.Apply();
		return texture;
	}
}