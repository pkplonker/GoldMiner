//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System.Collections.Generic;
using TerrainGeneration;
using UnityEngine;

namespace Props
{
	/// <summary>
	///PlayerSpawner full description
	/// </summary>
	public class SingleItemSpawner : MonoBehaviour
	{
		[SerializeField] private List<SingleInstanceSpawn> prefabsToSpawn;
		private static List<GameObject> spawnedPrefabs = new();

		private void OnEnable()
		{
			var mapGenerator = ServiceLocator.Instance.GetService<MapGenerator>();
			mapGenerator.TerrainGenerated += SpawnAll;
			mapGenerator.MapGenerationStarted += DespawnObjects;
		}

		private void OnDisable()
		{
			var mapGenerator = ServiceLocator.Instance.GetService<MapGenerator>();
			mapGenerator.TerrainGenerated -= SpawnAll;
			mapGenerator.MapGenerationStarted -= DespawnObjects;
		}

		private void DespawnObjects(int notRequired = 0, int notRequired2 = 0)
		{
			if (spawnedPrefabs == null || spawnedPrefabs.Count == 0) return;
			foreach (var spawned in spawnedPrefabs)
			{
				DestroyObject(spawned);
			}
		}

		private static void DestroyObject(GameObject spawned) => Destroy(spawned);

		private void SpawnAll(MapData mapData)
		{
			DespawnObjects();

			foreach (var p in prefabsToSpawn)
			{
				Spawn(p, mapData);
			}
		}

		protected virtual void Spawn(SingleInstanceSpawn sis, MapData mapData)
		{
			if (sis.Spawn(mapData, out GameObject obj))
			{
				if (obj.transform == null)
					obj.transform.SetParent(transform);
				spawnedPrefabs.Add(obj);
			}
			else
			{
				HandleFailedSpawn(sis, mapData);
			}
		}

		private void HandleFailedSpawn(SingleInstanceSpawn sis, MapData mapData)
		{
			if (!sis.allowFailure)
				ServiceLocator.Instance.GetService<SceneHandler>()
					.HandleFailedGeneration(mapData);
		}
	}
}