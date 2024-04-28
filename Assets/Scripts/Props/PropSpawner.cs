using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.VersionControl;
using UnityEngine;
using System.Threading.Tasks;
using TerrainGeneration;
using Task = System.Threading.Tasks.Task;

public class PropSpawner : MonoBehaviour, IService
{
	[field: SerializeField]
	public PropCollection PropCollections { get; set; }

	public static event Action<int> OnPropGenerated;
	public static event Action<int> OnPropsGenerationStarted;

	public static event Action OnPropsGenerated;
	private List<Vector2> points;
	private Queue<PoissonData> poissonDataQueue;

	private WorldGenerator worldGenerator;

	private void Start()
	{
		ServiceLocator.Instance.RegisterService<PropSpawner>(this);
		worldGenerator = ServiceLocator.Instance.GetService<WorldGenerator>();
		worldGenerator.MapGenerated += WorldGeneratorOnMapGenerated;
	}

	private void WorldGeneratorOnMapGenerated()
	{
		Debug.Log("PropSpawner");
		StartCoroutine(SpawnObjectsCor(worldGenerator.MapData));
	}

	private IEnumerator SpawnObjectsCor(MarchingCubeMapData mapData)
	{
		var tasks = new List<Task>();
		for (var j = 0; j < PropCollections.Props.Count; j++)
		{
			var j1 = j;
			var spawnArea = PropCollections.Props[j].GetSpawnSize(mapData);
			int maxPointsPerProp =
				(int) ((spawnArea * spawnArea * PropCollections.Props[j1].MaxQuantityPer100M / 10000) * 1.1f);
			var task = Task.Run(() => PoissonDiscSampling.GeneratePoints(index: j1, maxPointsPerProp,
				new Vector2(spawnArea, spawnArea), worldGenerator.MapData, PoissonCallback,
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
			StartCoroutine(PropCollections.Props[data.Index].ProcessPointDataCor(data,
				PropSpawnCompleteCallback, this, worldGenerator.MapData));
			yield return null;
		}

		for (var i = 0; i < worldGenerator.ChunkManager.maxChunkCoord.x; i++)
		{
			for (var j = 0; j < worldGenerator.ChunkManager.maxChunkCoord.x; j++)
			{
				for (var k = 0; k < worldGenerator.ChunkManager.maxChunkCoord.z; k++)
				{
					GameObject terrainChunk = worldGenerator.ChunkManager.Chunks[i, j, k].gameObject;

					var childObjects = terrainChunk.GetComponentsInChildren<Transform>()
						.Select(x => x.gameObject)
						.Where(x => x != terrainChunk)
						.ToArray();

					if (childObjects.Length > 0)
					{
						StaticBatchingUtility.Combine(childObjects, terrainChunk);
					}
				}
			}
		}
	}

	public void SpawnProp(int index, Vector3 result, Quaternion rotation)
	{
		var parent = worldGenerator.ChunkManager.GetChunkFromPosition(result).transform;
		var go = Instantiate(PropCollections.Props[index].Prefab, parent != null ? parent : transform);
		go.transform.position = result;
		go.transform.rotation = rotation;
		go.isStatic = PropCollections.Props[index].StaticObject;
	}

	private int count;

	private void PropSpawnCompleteCallback()
	{
		count++;
		OnPropGenerated?.Invoke(count);
		if (count == PropCollections.Props.Count - 1)
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

	public void Initialize() { }
}