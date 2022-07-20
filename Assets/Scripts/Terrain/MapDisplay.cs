using System;
using UnityEngine;

namespace Terrain
{
	public class MapDisplay : MonoBehaviour
	{
		[SerializeField] private Renderer renderer;


		public void DrawNoiseMap(float[,] noiseMap)
		{
			var width = noiseMap.GetLength(0);
			var height = noiseMap.GetLength(1);
			Texture2D texture2D = new Texture2D(width, height);

			Color[] colorMap = new Color[width * height];
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < height; x++)
				{
					colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
				}
			}

			texture2D.SetPixels(colorMap);
			texture2D.Apply();
			renderer.sharedMaterial.mainTexture = texture2D;
			renderer.transform.localScale = new Vector3(width, 1, height);
		}
	}
}