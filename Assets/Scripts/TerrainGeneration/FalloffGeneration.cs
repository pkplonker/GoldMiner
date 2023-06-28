using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class FalloffGeneration
{
	//best so far

	// public static float[,] GenerateFalloffMap(int size, int borderSize, int borderChangeDistance, int borderDistance)
	// {
	// 	if (borderSize + borderDistance + borderChangeDistance > (size / 2))
	// 	{
	// 		Debug.LogError("Invalid parameters");
	// 		throw new ArgumentOutOfRangeException("Invalid parameters");
	// 	}
	//
	// 	float[,] map = new float[size, size];
	//
	// 	for (int i = 0; i < size; i++)
	// 	{
	// 		for (int j = 0; j < size; j++)
	// 		{
	// 			float x = i;
	// 			float y = j;
	//
	// 			// Compute the distances to the edges
	// 			float xDistance = Mathf.Min(x, size - x);
	// 			float yDistance = Mathf.Min(y, size - y);
	// 			float minDistance = Mathf.Min(xDistance, yDistance);
	//
	// 			if (minDistance <= borderDistance) //outside
	// 			{
	// 				map[i, j] = 0;
	// 			}
	// 			else if (minDistance <= borderDistance + borderChangeDistance / 2) // going down
	// 			{
	// 				float t = Mathf.InverseLerp(borderDistance, borderDistance + borderChangeDistance / 2, minDistance);
	// 				map[i, j] = t;
	// 			}
	// 			else if (minDistance <= borderDistance + borderChangeDistance / 2 + borderSize) // flat
	// 			{
	// 				map[i, j] = 1.0f;
	// 			}
	// 			else if (minDistance <= borderDistance + borderChangeDistance + borderSize) //going up
	// 			{
	// 				float t = Mathf.InverseLerp(borderDistance + borderChangeDistance / 2 + borderSize,
	// 					borderDistance + borderChangeDistance + borderSize, minDistance);
	// 				map[i, j] = 1 - t;
	// 			}
	// 			else //normal
	// 			{
	// 				map[i, j] = 0;
	// 			}
	// 		}
	// 	}
	//
	// 	return map;
	// }

	//better
	// public static float[,] GenerateFalloffMap(int size, int borderSize, int borderChangeDistance, int borderDistance)
	// {
	// 	if (borderSize + borderDistance + borderChangeDistance > (size / 2))
	// 	{
	// 		Debug.LogError("Invalid parameters");
	// 		throw new ArgumentOutOfRangeException("Invalid parameters");
	// 	}
	//
	// 	float[,] map = new float[size, size];
	//
	// 	for (int i = 0; i < size; i++)
	// 	{
	// 		for (int j = 0; j < size; j++)
	// 		{
	// 			float x = i;
	// 			float y = j;
	//
	// 			// Compute the distances to the edges
	// 			float xDistance = Mathf.Min(x, size - x);
	// 			float yDistance = Mathf.Min(y, size - y);
	// 			float minDistance = Mathf.Min(xDistance, yDistance);
	//
	// 			if (minDistance <= borderDistance) //outside
	// 			{
	// 				map[i, j] = 0;
	// 			}
	// 			else if (minDistance <= borderDistance + borderChangeDistance / 2) // going down
	// 			{
	// 				// Interpolate from 0 to the average
	// 				float t = Mathf.InverseLerp(borderDistance, borderDistance + borderChangeDistance / 2, minDistance);
	// 				map[i, j] = t / 2f; // Adjusted to match the average height at the start of "flat"
	// 			}
	// 			else if (minDistance <= borderDistance + borderChangeDistance / 2 + borderSize) // flat
	// 			{
	// 				map[i, j] = 0.5f; // Flat area, average falloff
	// 			}
	// 			else if (minDistance <= borderDistance + borderChangeDistance + borderSize) //going up
	// 			{
	// 				// Interpolate from the average to 0
	// 				float t = Mathf.InverseLerp(borderDistance + borderChangeDistance / 2 + borderSize, borderDistance + borderChangeDistance + borderSize, minDistance);
	// 				map[i, j] = 0.5f * (1 - t); // Adjusted to match the average height at the end of "flat"
	// 			}
	// 			else //normal
	// 			{
	// 				map[i, j] = 0; // Normal terrain, no falloff
	// 			}
	// 		}
	// 	}
	//
	// 	return map;
	// }

	//backup
	// public static float[,] GenerateFalloffMap(int size, int borderSize, int borderChangeDistance, int borderDistance)
	// {
	// 	if (borderSize + borderDistance + borderChangeDistance > (size / 2))
	// 	{
	// 		Debug.LogError("Invalid parameters");
	// 		throw new ArgumentOutOfRangeException("Invalid parameters");
	// 	}
	//
	// 	float[,] map = new float[size, size];
	//
	// 	for (int i = 0; i < size; i++)
	// 	{
	// 		for (int j = 0; j < size; j++)
	// 		{
	// 			float x = i;
	// 			float y = j;
	//
	// 			// Compute the distances to the edges
	// 			float xDistance = Mathf.Min(x, size - x);
	// 			float yDistance = Mathf.Min(y, size - y);
	// 			float minDistance = Mathf.Min(xDistance, yDistance);
	//
	// 			if (minDistance <= borderDistance) //outside
	// 			{
	// 				map[i, j] = 0;
	// 			}
	// 			else if (minDistance <= borderDistance + borderChangeDistance / 2) // going down
	// 			{
	// 				// Interpolate from 0 to 0.5 (Average of 0 and 1)
	// 				float t = Mathf.InverseLerp(borderDistance, borderDistance + borderChangeDistance / 2, minDistance);
	// 				map[i, j] = t * 0.5f;
	// 			}
	// 			else if (minDistance <= borderDistance + borderChangeDistance / 2 + borderSize) // flat
	// 			{
	// 				map[i, j] = 0.5f; // Flat area, average falloff
	// 			}
	// 			else if (minDistance <= borderDistance + borderChangeDistance + borderSize) //going up
	// 			{
	// 				// Interpolate from 0.5 (Average of 0 and 1) to 0
	// 				float t = Mathf.InverseLerp(borderDistance + borderChangeDistance / 2 + borderSize,
	// 					borderDistance + borderChangeDistance + borderSize, minDistance);
	// 				map[i, j] = 0.5f * (1 - t);
	// 			}
	// 			else //normal
	// 			{
	// 				map[i, j] = 0; // Normal terrain, no falloff
	// 			}
	// 		}
	// 	}
	//
	// 	return map;
	// }

	public static float[,] GenerateFalloffMap(int size, int borderSize, int borderChangeDistance, int borderDistance)
	{
		if (borderSize + borderDistance + borderChangeDistance > (size / 2))
		{
			Debug.LogError("Invalid parameters");
			throw new ArgumentOutOfRangeException("Invalid parameters");
		}

		float[,] map = new float[size, size];
		float average = 0;
		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				average += map[i, j];
			}
		}

		average /= map.Length;

		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				float x = i;
				float y = j;

				// Compute the distances to the edges
				float xDistance = Mathf.Min(x, size - x);
				float yDistance = Mathf.Min(y, size - y);
				float minDistance = Mathf.Min(xDistance, yDistance);

				if (minDistance <= borderDistance) //outside
				{
					map[i, j] = 0;
				}
				else if (minDistance <= borderDistance + borderChangeDistance / 2) // going down
				{
					// Interpolate from 0 to 0.5 (Average of 0 and 1)
					float t = Mathf.InverseLerp(borderDistance, borderDistance + borderChangeDistance / 2, minDistance);
					map[i, j] = t * 0.5f;
				}
				else if (minDistance <= borderDistance + borderChangeDistance / 2 + borderSize) // flat
				{
					map[i, j] = 0.5f; // Flat area, average falloff
				}
				else if (minDistance <= borderDistance + borderChangeDistance + borderSize) //going up
				{
					// Interpolate from 0.5 (Average of 0 and 1) to 0
					float t = Mathf.InverseLerp(borderDistance + borderChangeDistance / 2 + borderSize,
						borderDistance + borderChangeDistance + borderSize, minDistance);
					map[i, j] = 0.5f * (1 - t);
				}
				else //normal
				{
					map[i, j] = 0; // Normal terrain, no falloff
				}
			}
		}

		return map;
	}
}