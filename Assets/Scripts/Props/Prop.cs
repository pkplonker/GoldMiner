using System;
using System.Collections;
using System.Collections.Generic;
using StuartHeathTools;
using TerrainGeneration;
using UnityEngine;
using UnityEngine.Serialization;

namespace Props
{
	[Serializable]
	public abstract class Prop : ScriptableObject
	{
		[SerializeField] public bool StaticObject = true;
		[SerializeField] public bool Spawn = true;

		[SerializeField] public GameObject Prefab;
		[Range(0, 40), SerializeField] public int NumSamplesBeforeRejection = 5;
		[Range(0, 2500), SerializeField] public int MaxQuantityPer100M = 100;

		[Range(-1f, 1f), SerializeField] public float FlatnessTolerance = 0.1f;

		[Range(0.1f, 10f), SerializeField] public float Radius;

		[SerializeField] public bool OverrideRideRadius = false;
		private const float THRESHOLD = 100f;
		[SerializeField] public bool InBoundryOnly = false;

		public virtual float GetRadius()
		{
			if (OverrideRideRadius) return Radius;
			Renderer rend = null;
			if (Prefab.TryGetComponent(out Renderer render)) rend = render;
			else if (Prefab.TryGetComponent(out LODGroup lgroup))
			{
				if (lgroup.lodCount > 0) rend = lgroup.GetLODs()[0].renderers[0];
			}
//de
			if (rend == null) return float.PositiveInfinity;

			var size = rend.bounds.size;
			return Mathf.Sqrt(size.x * size.x + size.z * size.z) / 2;
		}

		protected float GetTolerance() => FlatnessTolerance + FlatnessTolerance;

		public IEnumerator ProcessPointDataCor(PoissonData poissonData,
			Action callback, PropSpawner propSpawner, MapData mapData)
		{
			var index = poissonData.Index;

			if (!Spawn)
			{
				callback?.Invoke();
				yield break;
			}

			var points = poissonData.Points;

			var prng = new System.Random(mapData.seed);
			points.ShuffleWithPRNG(prng);
			var numToSpawn = CalculateNumberToSpawn(mapData, points);
			var tolerance = GetTolerance();
			int spawnedPointsThisCycle = 0;
			for (var i = 0; i < points.Count; i++)
			{
				if (spawnedPointsThisCycle > THRESHOLD)
				{
					yield return 0;
					spawnedPointsThisCycle = 0;
				}

				if (numToSpawn <= 0) break;
				if (!CalculatePlacement(mapData, points, i, tolerance, out var result, out var rotation)) continue;
				if (result.IsInfinity()) continue;
				propSpawner.SpawnProp(index, result, rotation);
				numToSpawn--;
			}

			// Debug.Log(
			// 	$"spawned {cachedNumberToSpawn - numToSpawn}/{cachedNumberToSpawn} {name} from {points.Count}");
			callback?.Invoke();
		}

		protected virtual int CalculateNumberToSpawn(MapData mapData, List<Vector2> points)
		{
			var result = (int) Mathf.Min(points.Count,
				MaxQuantityPer100M / 100f * (mapData.MapChunkSize * mapData.ChunksPerRow));
			return result;
		}

		protected virtual bool CalculatePlacement(MapData mapData, List<Vector2> points, int i, float tolerance,
			out Vector3 result,
			out Quaternion rotation)
		{
			result = CalculatePosition(new Vector3(points[i].x, 0, points[i].y),
				mapData);
			rotation = CalculateRotation(i, mapData.seed);
			if (result.IsInfinity()) return false;
			var bounds = BoundDrawer.GetBounds(Prefab);
			var isFlat = BoundDrawer.DetermineIfGeometryIsFlat(new BoundDrawer.GeometryFlatData(
				result - new Vector3(0, bounds.extents.y, 0),
				bounds, tolerance, mapData.GroundLayer, rotation));
			return isFlat;
		}

		protected static Quaternion CalculateRotation(int i, int seed)
		{
			var prng = new System.Random(seed + i);
			return Quaternion.Euler(0, prng.NextSingle(0, 360), 0);
		}

		protected virtual Vector3 CalculatePosition(Vector3 position, MapData mapData, float factor = 10)
		{
			position.y = mapData.HeightMultiplier;
			if (!Physics.Raycast(position, Vector3.down, out var hit, mapData.HeightMultiplier + factor,
				    LayerMask.GetMask(mapData.GroundLayer))) return Vector3.positiveInfinity;

			position.y = hit.point.y - GetDropIntoTerrainAmount(mapData.seed, position);
			return position;
		}

		protected virtual float GetDropIntoTerrainAmount(int seed, Vector3 position) => 0f;

		public virtual float GetSpawnSize(MapData mapData) =>
			InBoundryOnly ? mapData.GetSize() - (mapData.BoundaryInstep * 2) : mapData.GetSize();
	}
}