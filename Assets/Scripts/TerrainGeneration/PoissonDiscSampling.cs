using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace TerrainGeneration
{
	public static class PoissonDiscSampling
	{
		//Credit to Sebastian Lague for the original algorithm.
		public static void GeneratePointsCor(int index,float radius, Vector2 sampleRegionSize,
			 Action<PoissonData> callback, MapData mapData, int numSamplesBeforeRejection = 30)
		{
			Profiler.BeginSample("disc");
			var prng = new System.Random(mapData._seed+index);
			var cellSize = radius / Mathf.Sqrt(2);

			var grid = new int[Mathf.CeilToInt(sampleRegionSize.x / cellSize),
				Mathf.CeilToInt(sampleRegionSize.y / cellSize)];
			var points = new List<Vector2>();

			var spawnPoints = new List<Vector2> {sampleRegionSize / 2};
			while (spawnPoints.Count > 0)
			{
				var spawnIndex = (int) prng.NextSingle(0, spawnPoints.Count - 1);
				var spawnCentre = spawnPoints[spawnIndex];
				var candidateAccepted = false;


				for (var i = 0; i < numSamplesBeforeRejection; i++)
				{
					var angle = prng.NextSingle() * Mathf.PI * 2;
					Vector2 dir;
					dir.x = Mathf.Sin(angle);
					dir.y = Mathf.Cos(angle);
					var candidate = spawnCentre + dir * prng.NextSingle(radius, 2 * radius);
					if (!IsValid(candidate, sampleRegionSize, cellSize, radius, points, grid)) continue;
					points.Add(candidate);
					spawnPoints.Add(candidate);
					grid[(int) (candidate.x / cellSize), (int) (candidate.y / cellSize)] = points.Count;
					candidateAccepted = true;
					break;
				}

				if (!candidateAccepted) spawnPoints.RemoveAt(spawnIndex);
			}

			callback?.Invoke(new PoissonData(index,points));
			Profiler.EndSample();
		}

		static bool IsValid(Vector2 candidate, Vector2 sampleRegionSize, float cellSize, float radius,
			IReadOnlyList<Vector2> points, int[,] grid)
		{
			if (!(candidate.x >= 0) || !(candidate.x < sampleRegionSize.x) || !(candidate.y >= 0) ||
			    !(candidate.y < sampleRegionSize.y)) return false;

			var cellX = (int) (candidate.x / cellSize);
			var cellY = (int) (candidate.y / cellSize);
			var searchStartX = Mathf.Max(0, cellX - 2);
			var searchEndX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
			var searchStartY = Mathf.Max(0, cellY - 2);
			var searchEndY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);
			for (var x = searchStartX; x <= searchEndX; x++)
			{
				for (var y = searchStartY; y <= searchEndY; y++)
				{
					var pointIndex = grid[x, y] - 1;
					if (pointIndex == -1) continue;
					var sqrDst = (candidate - points[pointIndex]).sqrMagnitude;
					if (sqrDst < radius * radius) return false;
				}
			}

			return true;
		}
	}
}