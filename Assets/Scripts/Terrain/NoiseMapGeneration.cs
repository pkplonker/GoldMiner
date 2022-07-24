using UnityEngine;

namespace Terrain
{
	public static class Noise
	{
		public static float[,] GenerateNoiseMap(int mapDepth, int mapWidth, float scale, float offsetX, float offsetZ,
			int octaves, float persistance, float lacunarity, int seed)
		{
			float[,] noiseMap = new float[mapDepth, mapWidth];
			for (int zIndex = 0; zIndex < mapDepth; zIndex++)
			{
				for (int xIndex = 0; xIndex < mapWidth; xIndex++)
				{
					
					float amplitude = 1;
					float frequency = 1;
					float noiseHeight = 0;
					// calculate sample indices based on the coordinates, the scale and the offset
					float sampleX = (xIndex + offsetX) / scale;
					float sampleZ = (zIndex + offsetZ) / scale;
					float noise = 0f;
					float normalization = 0f;
					
					for (int i = 0; i < octaves; i++)
					{
						noise += amplitude * Mathf.PerlinNoise(sampleX * frequency + seed, sampleZ * frequency + seed);
						normalization += amplitude;
						amplitude *= persistance;
						frequency *= lacunarity;
					}


					noise /= normalization;
					noiseMap[zIndex, xIndex] = noise;
				}
			}

			return noiseMap;
		}
	}
}