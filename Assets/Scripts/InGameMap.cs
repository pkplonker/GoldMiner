using System;
using UnityEngine;
using System.IO;
using TerrainGeneration;

[RequireComponent(typeof(MapGeneratorTerrain))]
public class InGameMap : MonoBehaviour
{
	[field: SerializeField] public Texture2D CombinedTexture { get; private set; }
	private MapGeneratorTerrain mapGeneratorTerrain;
	public static event Action<Texture2D> OnMapGenerated;

	// private static void WriteToFile(Texture2D combinedTexture)
	// {
	// 	var pngData = combinedTexture.EncodeToPNG();
	// 	var savePath = Path.Combine(Application.persistentDataPath, "CombinedTexture.png");
	// 	File.WriteAllBytes(savePath, pngData);
	// 	//Debug.Log("Combined texture saved as PNG: " + savePath);
	// }

	private int originalSize = 0;

	private void Start()
	{
		MapGenerator.MapGenerated += MapGeneratorOnMapGenerated;
		GoldSpawnManager.Instance.GoldDeregistered += OnGoldDeregistered;
		mapGeneratorTerrain = GetComponent<MapGeneratorTerrain>();
	}

	private void OnDisable()
	{
		MapGenerator.MapGenerated -= MapGeneratorOnMapGenerated;
		GoldSpawnManager.Instance.GoldDeregistered -= OnGoldDeregistered;
	}

	private void MapGeneratorOnMapGenerated(float obj)
	{
		var size = MapGeneratorTerrain.terrainChunks.GetLength(0);
		var colorMapSize = (int) Mathf.Sqrt(MapGeneratorTerrain.terrainChunkData[0, 0].ColourMap.Length);
		originalSize = size * colorMapSize;
		var combinedColorMap = new Color[originalSize * originalSize];
		for (var x = 0; x < size; x++)
		{
			for (var y = 0; y < size; y++)
			{
				var chunkData = MapGeneratorTerrain.terrainChunkData[x, y];
				var offsetX = x * colorMapSize;
				var offsetY = y * colorMapSize;
				for (var i = 0; i < colorMapSize; i++)
				{
					for (var j = 0; j < colorMapSize; j++)
					{
						combinedColorMap[(offsetY + j) * originalSize + offsetX + i] =
							chunkData.ColourMap[j * colorMapSize + i];
					}
				}
			}
		}

		CombinedTexture = new Texture2D(originalSize, originalSize);
		CombinedTexture.SetPixels(combinedColorMap);
		originalSize /= (mapGeneratorTerrain.MapData.lod);
		CombinedTexture.Apply();
		CombinedTexture = ResizeTexture(CombinedTexture, 1024, 1024);
		//WriteToFile(CombinedTexture);
		OnMapGenerated?.Invoke(CombinedTexture);
	}

	private Texture2D ResizeTexture(Texture2D sourceTexture, int targetWidth, int targetHeight)
	{
		var rt = RenderTexture.GetTemporary(targetWidth, targetHeight);
		Graphics.Blit(sourceTexture, rt);
		var resizedTexture = new Texture2D(targetWidth, targetHeight);
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