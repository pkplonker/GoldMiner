//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System.Collections.Generic;
using TerrainGeneration;
using UnityEngine;

namespace Player
{
	/// <summary>
	///PlayerSpawner full description
	/// </summary>
	public class SingleItemSpawner : MonoBehaviour
	{
		[SerializeField] private string groundLayer = "Ground";
		[SerializeField] private List<SingleInstanceSpawn> prefabsToSpawn;
		private static List<GameObject> spawnedPrefabs = new();

		private void OnEnable()
		{
			MapGenerator.TerrainGenerated += SpawnAll;
			MapGenerator.MapGenerationStarted += DespawnObjects;
		}

		private void OnDisable()
		{
			MapGenerator.TerrainGenerated -= SpawnAll;
			MapGenerator.MapGenerationStarted -= DespawnObjects;
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

		private void SpawnAll(float size)
		{
			DespawnObjects();

			foreach (var p in prefabsToSpawn)
			{
				Spawn(p, size);
			}
		}

		protected virtual void Spawn(SingleInstanceSpawn sis, float size)
		{
			var currentInstance = Instantiate(sis.Prefab);
			currentInstance.SetActive(false);
			var spawnTransform = sis.CalculateSpawn(size, currentInstance, groundLayer);

			if (spawnTransform.Position.x < size)
			{
				currentInstance.transform.position = spawnTransform.Position;
				currentInstance.transform.rotation = spawnTransform.Rotation;
				currentInstance.SetActive(true);
				spawnedPrefabs.Add(currentInstance);
				sis.Setup(currentInstance);
			}
			else
			{
				HandleFailedSpawn(sis);
			}
		}

		private void HandleFailedSpawn(SingleInstanceSpawn sis) => Debug.LogError($"Failed to spawn {sis.Prefab.name}");
	}
}