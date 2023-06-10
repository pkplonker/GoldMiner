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
		[field: SerializeField] public bool StaticObject = true;
		[field: SerializeField] public bool Spawn { get; protected set; }

		[field: SerializeField] public GameObject Prefab { get; protected set; }
		[field: Range(0, 40), SerializeField] public int NumSamplesBeforeRejection { get; protected set; }
		[field: Range(0, 500), SerializeField] public int MaxQuantityPer100M { get; protected set; } = 100;

		[field: Range(-1f, 1f), SerializeField]
		public float FlatnessTolerance { get; protected set; }

		[field: Range(0.1f, 10f), SerializeField]
		public float Radius { get; protected set; }

		[field: SerializeField] public bool OverrideRideRadius { get; protected set; }

		public virtual float GetRadius()
		{
			if (OverrideRideRadius) return Radius;
			Renderer rend = null;
			if (Prefab.TryGetComponent(out Renderer render)) rend = render;
			else if (Prefab.TryGetComponent(out LODGroup lgroup))
			{
				if (lgroup.lodCount > 0) rend = lgroup.GetLODs()[0].renderers[0];
			}

			if (rend == null) return float.PositiveInfinity;

			var size = rend.bounds.size;
			return Mathf.Sqrt(size.x * size.x + size.z * size.z) / 2;
		}

		protected float GetTolerance() => FlatnessTolerance + FlatnessTolerance;

		public IEnumerator ProcessPointDataCor(PoissonData poissonData, int currentAmount, int targetAmount,
			Action<int, int> callback, PropSpawner propSpawner, MapData mapData)
		{
			if (!Spawn)
			{
				callback?.Invoke(currentAmount, targetAmount);
				yield break;
			}
			var points = poissonData.Points;
			var cachedTime = Time.realtimeSinceStartup;
			const float ALLOWED_TIME_PER_FRAME = 1 / 45f;
			var index = poissonData.Index;
			

			var prng = new System.Random(mapData.seed);
			points.ShuffleWithPRNG(prng);
			var numToSpawn = CalculateNumberToSpawn(mapData, points);
			var tolerance = GetTolerance();
			for (var i = 0; i < points.Count; i++)
			{
				if (Time.realtimeSinceStartup - cachedTime >= ALLOWED_TIME_PER_FRAME)
				{
					yield return 0;
					cachedTime = Time.realtimeSinceStartup;
				}

				if (numToSpawn <= 0) break;
				if (!CalculatePlacement(mapData, points, i, tolerance, out var result, out var rotation)) continue;

				propSpawner.SpawnProp(index, result, rotation);
				numToSpawn--;
			}

			if (numToSpawn > 0)
			{
				Debug.LogWarning($"spawned {targetAmount-numToSpawn}/{targetAmount} {name} from {points.Count}");
			}
			callback?.Invoke(currentAmount, targetAmount);
		}

		protected virtual int CalculateNumberToSpawn(MapData mapData, List<Vector2> points) =>
			(int) Mathf.Min(points.Count,
				MaxQuantityPer100M / 100f * (mapData.MapChunkSize * mapData.ChunksPerRow));

		protected virtual bool CalculatePlacement(MapData mapData, List<Vector2> points, int i, float tolerance,
			out Vector3 result,
			out Quaternion rotation)
		{
			result = CalculatePosition(new Vector3(points[i].x, 0, points[i].y),
				mapData);
			rotation = CalculateRotation(i, mapData.seed);

			if (result == Vector3.positiveInfinity) return true;
			var bounds = BoundDrawer.GetBounds(Prefab);
			var isFlat = BoundDrawer.DetermineIfGeometryIsFlat(new BoundDrawer.GeometryFlatData(
				result - new Vector3(0, bounds.extents.y, 0),
				bounds, tolerance, mapData.terrainLayer, rotation));
			Debug.Log($"isFlat: {isFlat}");
			return isFlat;
		}

		protected static Quaternion CalculateRotation(int i, int seed)
		{
			var prng = new System.Random(seed + i);
			return Quaternion.Euler(0, prng.NextSingle(0, 360), 0);
		}

		protected virtual Vector3 CalculatePosition(Vector3 position, MapData mapData, float factor = 10)
		{
			position.y = mapData.heightMultiplier;
			if (!Physics.Raycast(position, Vector3.down, out var hit, mapData.heightMultiplier + factor,
				    LayerMask.GetMask(mapData.terrainLayer))) return Vector3.positiveInfinity;

			position.y = hit.point.y - GetDropIntoTerrainAmount(mapData.seed, position);
			return position;
		}

		protected virtual float GetDropIntoTerrainAmount(int seed, Vector3 position) => 0f;
	}
}