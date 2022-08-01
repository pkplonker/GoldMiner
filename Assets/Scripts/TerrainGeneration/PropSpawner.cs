using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using StuartHeathTools;
using UnityEngine;
using UnityEngine.Profiling;

namespace TerrainGeneration
{
	public class PropSpawner : MonoBehaviour
	{
		[SerializeField] private List<PropData> propDatas;
		public Transform container { get; private set; }
		public static event Action OnPropsGenerated;
		private List<Vector2> points;
		private Queue<PoissonData> poissonDataQueue;
		private MapData mapData;

		public void SpawnObjects(int spawnArea, MapData mapData)
		{
			this.mapData = mapData;
			if (container == null)
			{
				container = new GameObject
				{
					name = "PropContainer"
				}.transform;
				container.SetParent(transform);
				container.localPosition = transform.position;
			}

			poissonDataQueue = new Queue<PoissonData>();
			StartCoroutine(SpawnObjectsCor(spawnArea));
		}


		IEnumerator SpawnObjectsCor(int spawnArea)
		{
			List<Task> tasks = new List<Task>();
			for (var j = 0; j < propDatas.Count; j++)
			{
				var radius = propDatas[j].GetRadius();
				var radius1 = radius;
				var j1 = j;
				tasks.Add(Task.Run(() => PoissonDiscSampling.GeneratePointsCor(j1, radius1,
					new Vector2(spawnArea, spawnArea), PoissonCallback, mapData,
					propDatas[j1].numSamplesBeforeRejection)));
			}

			var targetAmount = propDatas.Count;
			var currentAmount = 0;
			while (currentAmount != targetAmount)
			{
				while (poissonDataQueue.Count == 0)
				{
					yield return null;
				}

				currentAmount++;

				StartCoroutine(ProcessPointDataCor(poissonDataQueue.Dequeue(),currentAmount,targetAmount));
				yield return null;
			}

			StaticBatchingUtility.Combine(container.gameObject);
		}
		

		private IEnumerator ProcessPointDataCor(PoissonData poissonData,int currentAmount, int targetAmount)
		{
			var cachedTime = Time.realtimeSinceStartup;
			var allowedTimePerFrame = 1 / 45f;
			var index = poissonData.index;
			points = poissonData.points;
			if (!propDatas[index].spawn) yield break;

			var prng = new System.Random(mapData.seed);
			points.ShuffleWithPRNG(prng);
			var numToSpawn = Mathf.Min(points.Count,
				propDatas[index].maxQuantityPer100M / 100f * (mapData.mapChunkSize * mapData.chunksPerRow));
			var tolerance = propDatas[index].GetTolerance();
			for (var i = 0; i < points.Count; i++)
			{
				if (Time.realtimeSinceStartup - cachedTime >= allowedTimePerFrame)
				{
					yield return 0;
					cachedTime = Time.realtimeSinceStartup;
				}

				if (numToSpawn <= 0)
					break;
				var result = CalculatePosition(new CalculatePositionData(new Vector3(points[i].x, 0, points[i].y),
					mapData, propDatas[index]));
				if (result == Vector3.positiveInfinity) continue;

				var rotation = CalculateRotation(i);

				var bounds = BoundDrawer.GetBounds(propDatas[index].prefab);


				if (!BoundDrawer.DetermineIfGeometryIsFlat(new BoundDrawer.GeometryFlatData(
					    result - new Vector3(0, bounds.extents.y, 0),
					    bounds, tolerance, mapData.terrainLayer, rotation))) continue;


				SpawnProp(index, result, rotation);

				numToSpawn--;
			}
			if(currentAmount==targetAmount)
				OnPropsGenerated?.Invoke();
		}

		private void SpawnProp(int index, Vector3 result, Quaternion rotation)
		{
			GameObject go;
			go = Instantiate(propDatas[index].prefab, container);
			go.transform.localPosition = result;
			go.transform.localRotation = rotation;
			go.isStatic = propDatas[index].staticObject;
		}

		private void PoissonCallback(PoissonData data)
		{
			lock (poissonDataQueue)
			{
				poissonDataQueue.Enqueue(data);
			}
		}

		private Quaternion CalculateRotation(int i)
		{
			var prng = new System.Random(mapData.seed + i);
			return Quaternion.Euler(0, prng.NextSingle(0, 360), 0);
		}

		private static Vector3 CalculatePosition(CalculatePositionData d)
		{
			d.position.y = d.mapData.heightMultiplier;

			if (!Physics.Raycast(d.position, Vector3.down, out RaycastHit hit, d.mapData.heightMultiplier + d.factor,
				    LayerMask.GetMask(d.mapData.terrainLayer))) return Vector3.positiveInfinity;

			d.position.y = hit.point.y - d.prop.dropIntoTerrainAmount;
			var normalisedHeight = d.position.y / d.mapData.heightMultiplier;
			return (normalisedHeight > d.prop.minHeightNormalised && normalisedHeight < d.prop.maxHeightNormalised)
				? d.position
				: Vector3.positiveInfinity;
		}

		private struct CalculatePositionData
		{
			internal Vector3 position;
			internal MapData mapData;
			public PropData prop;
			public float factor;

			public CalculatePositionData(Vector3 position, MapData mapData, PropData prop, float factor = 10)
			{
				this.position = position;
				this.mapData = mapData;
				this.prop = prop;
				this.factor = factor;
			}
		}
	}

	public struct PoissonData
	{
		public int index;
		public List<Vector2> points;

		public PoissonData(int index, List<Vector2> points)
		{
			this.index = index;
			this.points = points;
		}
	}

	[Serializable]
	public struct PropData
	{
		[field: SerializeField] public bool staticObject;
		[field: SerializeField] public bool spawn { get; private set; }

		[field: SerializeField] public GameObject prefab { get; private set; }
		[field: Range(0, 40), SerializeField] public int numSamplesBeforeRejection { get; private set; }
		[field: Range(0, 500), SerializeField] public int maxQuantityPer100M { get; private set; }
		[field: Range(0, 1f), SerializeField] public float minHeightNormalised { get; private set; }
		[field: Range(0, 1f), SerializeField] public float maxHeightNormalised { get; private set; }

		[field: Range(-1f, 1f), SerializeField]
		public float dropIntoTerrainAmount { get; private set; }

		[field: Range(-1f, 1f), SerializeField]
		public float flatnessTolerance { get; private set; }

		[field: Range(0.1f, 10f), SerializeField]
		public float radius { get; private set; }

		[field: SerializeField] public bool overrideRideRadius { get; private set; }

		public float GetRadius()
		{
			if (overrideRideRadius) return radius;
			Renderer rend = null;
			if (prefab.TryGetComponent(out Renderer render)) rend = render;
			else if (prefab.TryGetComponent(out LODGroup lgroup))
			{
				if (lgroup.lodCount > 0) rend = lgroup.GetLODs()[0].renderers[0];
			}

			if (rend == null) return float.PositiveInfinity;

			var size = rend.bounds.size;
			return Mathf.Sqrt(size.x * size.x + size.z * size.z) / 2;
		}

		public float GetTolerance() => flatnessTolerance + flatnessTolerance;
	}
}