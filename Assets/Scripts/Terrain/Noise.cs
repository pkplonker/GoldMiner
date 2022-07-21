using System;
using UnityEngine;

namespace Terrain
{
	public static class Noise
	{
		public static float[,] GenerateNoiseMap(int width, int height,int seed, float scale, int octaves,
			float persistance, float lacunarity, Vector2 offset, int vertexCountMultiplier)
		{
			float[,] noiseMap = new float[width, height];
			System.Random rng = new System.Random(seed);
			Vector2[] octaveOffsets = new Vector2[octaves];
			for (int i = 0; i < octaves; i++)
			{
				float offsetX = rng.Next(-100000, 100000)+offset.x;
				float offsetY = rng.Next(-100000, 100000)+offset.y;
				octaveOffsets[i] = new Vector2(offsetX, offsetY);
			}

			if (scale <= 0) scale = 0.00001f;

			float halfSize = width / 2f;
			var maxNoise = float.MinValue;
			var minNoise = float.MaxValue;
			for (int y = 0; y < width; y++)
			{
				for (int x = 0; x < width; x++)
				{
					float amplitude = 1;
					float frequency = 1;
					float noiseHeight = 0;
					for (int i = 0; i < octaves; i++)
					{
						float sampleX = ((x-halfSize) / scale * frequency+octaveOffsets[i].x)/vertexCountMultiplier;
						float sampleY = ((y-halfSize) / scale * frequency+octaveOffsets[i].y)/vertexCountMultiplier;
						float perlinValue = (Mathf.PerlinNoise(sampleX, sampleY) * 2) - 1;
						noiseHeight += perlinValue * amplitude;
						amplitude *= persistance;
						frequency *= lacunarity;
					}

					if (noiseHeight > maxNoise)
					{
						maxNoise = noiseHeight;
					}
					else if (noiseHeight < minNoise)
					{
						minNoise = noiseHeight;
					}

					noiseMap[x, y] = noiseHeight;
				}
			}

			for (int y = 0; y < width; y++)
			{
				for (int x = 0; x < width; x++)
				{
					noiseMap[x, y] = Mathf.InverseLerp(minNoise, maxNoise, noiseMap[x, y]);
				}
			}

			return noiseMap;
		}
	}
}