using System;
using UnityEngine;
using System.IO;
using TerrainGeneration;

[RequireComponent(typeof(MapGeneratorTerrain))]
public class InGameMap : MonoBehaviour
{
	[field: SerializeField] public Texture2D texture { get; private set; }
	public static event Action<Texture2D> OnMapGenerated;
	private int originalSize = 0;
	private float[,] noiseMap;
	private MapData mapData;
	[SerializeField] private float contourInterval = .25f;
	[SerializeField] private float contourRange = 0.1f;

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
		CheatConsole.Instance.RegisterCommand("Regenerate UI Map", Regenerate);
	}

	private void OnDisable()
	{
		MapGeneratorTerrain.OnNoiseMapGenerated -= OnNoiseMapGenerated;
		ServiceLocator.Instance.GetService<GoldSpawnManager>().GoldDeregistered -= OnGoldDeregistered;
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

		OnMapGenerated?.Invoke(texture);
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

	private void OnGoldDeregistered(Transform gold)
	{
		var goldPosition = gold.transform.position;
		var normalizedPosition = new Vector2(goldPosition.x / originalSize, goldPosition.z / originalSize);
		var scaledPosition = new Vector2(normalizedPosition.x * 1024, normalizedPosition.y * 1024);
		var textureCoordinate = new Vector2Int((int) scaledPosition.x, (int) scaledPosition.y);
	}
}