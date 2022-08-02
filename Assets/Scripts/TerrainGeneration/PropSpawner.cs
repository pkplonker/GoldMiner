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
		[SerializeField] private List<PropData> _propDatas;
		public Transform Container { get; private set; }
		public static event Action<int, int> OnPropGenerated;
		public static event Action<int> OnPropsGenerationStarted;

		public static event Action OnPropsGenerated;
		private List<Vector2> _points;
		private Queue<PoissonData> _poissonDataQueue;
		private MapData _mapData;

		public void SpawnObjects(int spawnArea, MapData mapData)
		{
			OnPropsGenerationStarted?.Invoke(_propDatas.Count);
			this._mapData = mapData;
			if (Container == null)
			{
				Container = new GameObject
				{
					name = "PropContainer"
				}.transform;
				Container.SetParent(transform);
				Container.localPosition = transform.position;
			}

			_poissonDataQueue = new Queue<PoissonData>();
			StartCoroutine(SpawnObjectsCor(spawnArea));
		}


		private IEnumerator SpawnObjectsCor(int spawnArea)
		{
			for (var j = 0; j < _propDatas.Count; j++)
			{
				var radius = _propDatas[j].GetRadius();
				var j1 = j;
				Task.Run(() => PoissonDiscSampling.GeneratePointsCor(j1, radius,
					new Vector2(spawnArea, spawnArea), PoissonCallback, _mapData,
					_propDatas[j1].NumSamplesBeforeRejection));
			}

			var targetAmount = _propDatas.Count;
			var currentAmount = 0;
			var data = _poissonDataQueue.Dequeue();
			while (currentAmount != targetAmount)
			{
				while (_poissonDataQueue.Count == 0)
				{
					yield return null;
				}

				currentAmount++;
				StartCoroutine(_propDatas[data.Index].ProcessPointDataCor(data, currentAmount, targetAmount,
					PropSpawnCompleteCallback, this, _mapData));
				yield return null;
			}

			StaticBatchingUtility.Combine(Container.gameObject);
		}

		public void SpawnProp(int index, Vector3 result, Quaternion rotation)
		{
			var go = Instantiate(_propDatas[index].Prefab, Container);
			go.transform.localPosition = result;
			go.transform.localRotation = rotation;
			go.isStatic = _propDatas[index]._staticObject;
		}

		private void PropSpawnCompleteCallback(int currentAmount, int targetAmount)
		{
			OnPropGenerated?.Invoke(currentAmount, targetAmount);
			if (currentAmount == targetAmount)
				OnPropsGenerated?.Invoke();
		}

		private void PoissonCallback(PoissonData data)
		{
			lock (_poissonDataQueue)
			{
				_poissonDataQueue.Enqueue(data);
			}
		}

		public int GetPropsRequired() => _propDatas.Count(p => p.Spawn);
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


