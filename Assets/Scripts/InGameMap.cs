using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Player;
using TerrainGeneration;

[RequireComponent(typeof(MapGeneratorTerrain))]
public class InGameMap : MonoBehaviour
{
	public enum MapUpdateType
	{
		Player,
		Gold,
		Target
	}

	[field: SerializeField] public Texture2D texture { get; private set; }
	public static event Action<InGameMap, Texture2D> OnMapGenerated;
	public static event Action<Vector2, MapUpdateType> PositionUpdate;

	private int originalSize = 0;
	private float[,] noiseMap;
	private MapData mapData;
	[SerializeField] private float contourInterval = .25f;
	[SerializeField] private float contourRange = 0.1f;
	[SerializeField] private Color goldHighlightColor = Color.green;
	[SerializeField] private Color targetHighlightColor = Color.blue;
	[SerializeField] private Color playerHighlightColor = Color.red;
	[SerializeField] private int playerMarkerSize = 7;
	[SerializeField] private int markerSize = 5;
	[SerializeField] private PlayerReference playerReference;
	private Texture2D playerTexture;
	private Texture2D targetTexture;

	private static void WriteToFile(Texture2D combinedTexture)
	{
		var pngData = combinedTexture.EncodeToPNG();
		var savePath = Path.Combine(Application.persistentDataPath, "texture.png");
		Debug.Log(savePath);
		File.WriteAllBytes(savePath, pngData);
	}

	private void Start()
	{
		MapGeneratorTerrain.OnNoiseMapGenerated += OnNoiseMapGenerated;
		ServiceLocator.Instance.GetService<GoldSpawnManager>().GoldDeregistered += OnGoldDeregistered;
		ServiceLocator.Instance.GetService<TargetManager>().TargetDeregistered += OnTargetDeregistered;

		CheatConsole.Instance.RegisterCommand("Regenerate UI Map", Regenerate);
	}

	private void OnDisable()
	{
		MapGeneratorTerrain.OnNoiseMapGenerated -= OnNoiseMapGenerated;
		ServiceLocator.Instance.GetService<GoldSpawnManager>().GoldDeregistered -= OnGoldDeregistered;
		ServiceLocator.Instance.GetService<TargetManager>().TargetDeregistered -= OnTargetDeregistered;
	}

	private void Regenerate()
	{
		if (noiseMap != null && mapData != null)
		{
			OnNoiseMapGenerated(noiseMap, mapData);
		}
	}

	private void OnNoiseMapGenerated(float[,] noiseMap, MapData mapData)
	{
		this.noiseMap = noiseMap;
		this.mapData = mapData;
		int width = noiseMap.GetLength(0);
		int height = noiseMap.GetLength(1);

		texture = new Texture2D(width, height);
		Color contourColor = Color.black; // Color of the contour lines

		AnimationCurve hc = mapData.HeightCurve;
		float heightMultiplier = mapData.HeightMultiplier;

		float minHeight = float.MaxValue;
		float maxHeight = float.MinValue;

		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				float sample = noiseMap[x, y];
				float heightValue = hc.Evaluate(sample) * heightMultiplier;
				minHeight = Mathf.Min(minHeight, heightValue);
				maxHeight = Mathf.Max(maxHeight, heightValue);
			}
		}

		Color lowestColor = new Color(0xA4 / 255f, 0x62 / 255f, 0x32 / 255f); // RGB for 0xA46232
		Color highestColor = new Color(0x6A / 255f, 0x2D / 255f, 0x09 / 255f); // RGB for 0x6A2D09
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				float sample = noiseMap[x, y];
				float heightValue = hc.Evaluate(sample) * heightMultiplier;

				heightValue = (heightValue - minHeight) / (maxHeight - minHeight);

				Color color = Color.Lerp(lowestColor, highestColor, heightValue);

				for (float contourHeight = contourInterval; contourHeight <= 1; contourHeight += contourInterval)
				{
					if (heightValue > contourHeight - contourRange && heightValue < contourHeight + contourRange)
					{
						color = contourColor;
						break;
					}
				}

				texture.SetPixel(x, y, color);
			}
		}

		texture.filterMode = FilterMode.Point;
		texture.Apply();

		OnMapGenerated?.Invoke(this, texture);
		WriteToFile(texture);
	}

	private Texture2D ResizeTexture(Texture2D sourceTexture, int targetWidth, int targetHeight)
	{
		var rt = RenderTexture.GetTemporary(targetWidth, targetHeight);
		Graphics.Blit(sourceTexture, rt);
		var resizedTexture = new Texture2D(targetWidth, targetHeight, TextureFormat.RGB24, false);
		resizedTexture.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
		resizedTexture.Apply();
		RenderTexture.ReleaseTemporary(rt);
		return resizedTexture;
	}

	private void OnGoldDeregistered(Transform obj) => GetPosition(obj, MapUpdateType.Gold);

	private void OnTargetDeregistered(Transform obj) => GetPosition(obj, MapUpdateType.Target);

	private void GetPosition(Transform t, MapUpdateType type)
	{
		if (targetTexture == null) return;
		var goldPosition = t.transform.position;

		var size = (mapData.GetSize() * mapData.LOD);
		var percentX = (goldPosition.x + size / 2f) / size;
		var percentY = (goldPosition.z + size / 2f) / size;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		var mapX = Mathf.RoundToInt((targetTexture.width - 1) * percentX);
		var mapY = Mathf.RoundToInt((targetTexture.height - 1) * percentY);

		// DrawCircle(targetTexture, mapX, mapY, markerSize, color);
		// texture.Apply();
		//OnMapGenerated?.Invoke(this, texture);
		PositionUpdate?.Invoke(new Vector2(mapX, mapY), type);
	}

	private void DrawCircle(Texture2D texture, int centerX, int centerY, int radius, Color color)
	{
		for (var y = -radius; y <= radius; y++)
		{
			for (var x = -radius; x <= radius; x++)
			{
				if (x * x + y * y > radius * radius) continue;
				var drawX = centerX + x;
				var drawY = centerY + y;
				if (drawX >= 0 && drawX < texture.width && drawY >= 0 && drawY < texture.height)
					texture.SetPixel(drawX, drawY, color);
			}
		}
	}

	public void UpdatePlayer()
	{
		if (playerReference == null || playerReference.GetPlayer() == null)
		{
			Debug.LogError("Player reference or player transform is not set.");
			return;
		}

		var playerPosition = playerReference.GetPlayer().transform;
		GetPosition(playerPosition, MapUpdateType.Player);
		// var size = (mapData.GetSize() * mapData.LOD);
		// var percentX = (playerPosition.x + size / 2f) / size;
		// var percentY = (playerPosition.z + size / 2f) / size;
		// percentX = Mathf.Clamp01(percentX);
		// percentY = Mathf.Clamp01(percentY);

		// var mapX = Mathf.RoundToInt((texture.width - 1) * percentX);
		// var mapY = Mathf.RoundToInt((texture.height - 1) * percentY);
		//
		// DrawCircle(texture, mapX, mapY, playerMarkerSize, playerHighlightColor);
		// texture.Apply();
		//OnMapGenerated?.Invoke(this, texture);
	}
}