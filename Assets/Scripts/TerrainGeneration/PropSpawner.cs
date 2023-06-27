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
		public static event Action<int> OnPropGenerated;
		public static event Action<int> OnPropsGenerationStarted;

		public static event Action OnPropsGenerated;
		private List<Vector2> points;
		private Queue<PoissonData> poissonDataQueue;
		private MapGeneratorTerrain mapGeneratorTerrain;
		public void SpawnObjects(MapGeneratorTerrain mapGeneratorTerrain)
		{
			this.mapGeneratorTerrain = mapGeneratorTerrain;
			OnPropsGenerationStarted?.Invoke(PropCollections.Props.Count);
			poissonDataQueue = new Queue<PoissonData>(PropCollections.Props.Count);
			OnPropsGenerationStarted?.Invoke(PropCollections.Props.Count);
			StartCoroutine(SpawnObjectsCor(mapGeneratorTerrain.MapData.GetSize()));
		}


		private IEnumerator SpawnObjectsCor(int spawnArea)
		{
			var tasks = new List<Task>();
			for (var j = 0; j < PropCollections.Props.Count; j++)
			{
				var j1 = j;
				var maxPointsPerProp = spawnArea * spawnArea / PropCollections.Props[j1].MaxQuantityPer100M;
				var task = Task.Run(() => PoissonDiscSampling.GeneratePointsCor(index: j1,  maxPointsPerProp,
					new Vector2(spawnArea, spawnArea), PoissonCallback, mapGeneratorTerrain.MapData,
					PropCollections.Props[j1].NumSamplesBeforeRejection));
				tasks.Add(task);
			}

			var numberOfDifferentPropsToSpawn = PropCollections.Props.Count;
			var index = 0;

			while (index != numberOfDifferentPropsToSpawn)
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
				//Debug.Log($"{data.Points.Count} found for {PropCollections.Props[data.Index].Prefab.name} ");
				StartCoroutine(PropCollections.Props[data.Index].ProcessPointDataCor(data, numberOfDifferentPropsToSpawn,
					PropSpawnCompleteCallback, this, mapGeneratorTerrain.MapData));
				yield return null;
			}

			for (var i = 0; i < MapGeneratorTerrain.terrainChunks.GetLength(0); i++)
			{
				for (var j = 0; j < MapGeneratorTerrain.terrainChunks.GetLength(1); j++)
				{
					StaticBatchingUtility.Combine(MapGeneratorTerrain.terrainChunks[i,j].gameObject);
				}
			}
		}


		public void SpawnProp(int index, Vector3 result, Quaternion rotation)
		{
			var parent = mapGeneratorTerrain.GetChunkFromPosition(result).transform;
			var go = Instantiate(PropCollections.Props[index].Prefab, parent != null ? parent : transform);
			go.transform.position = result;
			go.transform.rotation = rotation;
			go.isStatic = PropCollections.Props[index].StaticObject;
		}

		private int count;
		private void PropSpawnCompleteCallback()
		{
			//Debug.Log("prop spawn complete callback");
			count++;
			OnPropGenerated?.Invoke(count);
			if (count == PropCollections.Props.Count-1)
				OnPropsGenerated?.Invoke();
		}

		private void PoissonCallback(PoissonData data)
		{
			lock (poissonDataQueue)
			{
				//Debug.Log($"Poisson data generated for {data.Points.Count} points");
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


