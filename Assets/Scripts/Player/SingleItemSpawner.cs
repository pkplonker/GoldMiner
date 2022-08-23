//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using System.Collections.Generic;
using PlasticGui;
using TerrainGeneration;
using UnityEngine;

namespace Player
{
	/// <summary>
	///PlayerSpawner full description
	/// </summary>
	public class SingleItemSpawner : MonoBehaviour
	{
		[SerializeField] private string _groundLayer = "Ground";
		[SerializeField] private List<SingleInstanceSpawn> _prefabsToSpawn;
		private static List<GameObject> _spawnedPrefabs = new List<GameObject>();


		private void OnEnable()
		{
			MapGenerator.OnMapGenerated += SpawnAll;
			MapGenerator.OnMapGenerationStarted += DespawnObjects;
		}

		private void DespawnObjects(int notRequired = 0, int notRequired2 = 0)
		{
			if (_spawnedPrefabs == null || _spawnedPrefabs.Count == 0) return;
			foreach (var spawned in _spawnedPrefabs)
			{
				DestroyObject(spawned);
			}
		}

		private static void DestroyObject(GameObject spawned) => Destroy(spawned);


		private void SpawnAll(float size)
		{
			DespawnObjects();

			foreach (var p in _prefabsToSpawn)
			{
				Spawn(p, size);
			}
		}

		private void Spawn(SingleInstanceSpawn sis, float size)
		{
			var currentInstance = Instantiate(sis.Prefab);
			currentInstance.SetActive(false);
			var spawnTransform = sis.CalculateSpawn(size, currentInstance, _groundLayer);

			if (spawnTransform.Position.x < size)
			{
				currentInstance.transform.position = spawnTransform.Position;
				currentInstance.transform.rotation = spawnTransform.Rotation;
				currentInstance.SetActive(true);
				_spawnedPrefabs.Add(currentInstance);
				sis.Setup(currentInstance);
			}
			else HandleFailedSpawn(sis);
		}


		private void HandleFailedSpawn(SingleInstanceSpawn sis)=>Debug.LogError($"Failed to spawn {sis.Prefab.name}");
		

		private void OnDisable() => MapGenerator.OnMapGenerated -= SpawnAll;
	}
}