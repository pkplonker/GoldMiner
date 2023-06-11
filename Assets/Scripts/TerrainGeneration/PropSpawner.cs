using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace TerrainGeneration
{
	public class PropSpawner : MonoBehaviour
	{
		[SerializeField] private PropCollection PropCollections;
		public Transform Container { get; private set; }
		public static event Action<int, int> OnPropGenerated;
		public static event Action<int> OnPropsGenerationStarted;

		public static event Action OnPropsGenerated;
		private List<Vector2> points;
		private Queue<PoissonData> poissonDataQueue;
		private MapData mapData;

		public void SpawnObjects(int spawnArea, MapData mapData)
		{
			OnPropsGenerationStarted?.Invoke(PropCollections.Props.Count);
			this.mapData = mapData;
			if (Container == null)
			{
				Container = new GameObject
				{
					name = "PropContainer"
				}.transform;
				Container.SetParent(transform);
				Container.localPosition = transform.position;
			}

			poissonDataQueue = new Queue<PoissonData>(PropCollections.Props.Count);
			OnPropsGenerationStarted?.Invoke(PropCollections.Props.Count);
			StartCoroutine(SpawnObjectsCor(spawnArea));
		}


		private IEnumerator SpawnObjectsCor(int spawnArea)
		{
			var tasks = new List<Task>();
			for (var j = 0; j < PropCollections.Props.Count; j++)
			{
				var j1 = j;
				var task =Task.Run(() => PoissonDiscSampling.GeneratePointsCor(j1, PropCollections.Props[j1].GetRadius(),
					new Vector2(spawnArea, spawnArea), PoissonCallback, mapData,
					PropCollections.Props[j1].NumSamplesBeforeRejection));
				tasks.Add(task);
			}

			var targetAmount = PropCollections.Props.Count;
			var index = 0;

			while (index != targetAmount)
			{
				while (poissonDataQueue.Count == 0)
				{
					foreach (var task in tasks.Where(task => task.IsFaulted))
					{
						Debug.LogError(task.Exception);
						Debug.LogError("Task failed");
					}

					yield return null;
				}
				var data = poissonDataQueue.Dequeue();

				index++;
				StartCoroutine(PropCollections.Props[data.Index].ProcessPointDataCor(data, index, targetAmount,
					PropSpawnCompleteCallback, this, mapData));
				yield return null;
			}

			StaticBatchingUtility.Combine(Container.gameObject);
		}

		public void SpawnProp(int index, Vector3 result, Quaternion rotation)
		{
			var go = Instantiate(PropCollections.Props[index].Prefab, Container);
			go.transform.localPosition = result;
			go.transform.localRotation = rotation;
			go.isStatic = PropCollections.Props[index].StaticObject;
		}

		private void PropSpawnCompleteCallback(int currentAmount, int targetAmount)
		{
			OnPropGenerated?.Invoke(currentAmount, targetAmount);
			if (currentAmount == targetAmount)
				OnPropsGenerated?.Invoke();
		}

		private void PoissonCallback(PoissonData data)
		{
			lock (poissonDataQueue)
			{
				poissonDataQueue.Enqueue(data);
			}
		}

		public int GetPropsRequired() => PropCollections.Props.Count(p => p.Spawn);
	}
}

public struct PoissonData
{
	public readonly int Index;
	public readonly List<Vector2> Points;

	public PoissonData(int index, List<Vector2> points)
	{
		Index = index;
		Points = points;
	}
}


