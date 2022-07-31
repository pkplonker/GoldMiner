//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using TerrainGeneration;
using UnityEngine;

namespace Player
{
	/// <summary>
	///PlayerSpawner full description
	/// </summary>
	public class PlayerSpawner : MonoBehaviour
	{
		[SerializeField] private GameObject playerPrefab;
		[SerializeField] private Vector3 offset;

		[SerializeField] private string groundLayer = "Ground";
		private GameObject currentPlayer;
		
		private void OnEnable() => MapGenerator.OnMapGenerated += SpawnPlayer;
		
		private void SpawnPlayer(float size)
		{
			if (currentPlayer != null)
			{
				Destroy(currentPlayer);
			}
			currentPlayer = Instantiate(playerPrefab);

			var spawnPos = CalculateSpawnPosition(size) + offset;
			if (spawnPos != Vector3.positiveInfinity)
			{
				currentPlayer.transform.position = spawnPos;
				currentPlayer.GetComponent<PlayerMovement>().SetCanMove(true);
				
			}
			else HandleFailedSpawn();
		}

		private void HandleFailedSpawn()
		{
			currentPlayer.GetComponent<PlayerMovement>().SetCanMove(false);

			Debug.LogError("Failed to spawn player");
			throw new NotImplementedException();
		}


		private Vector3 CalculateSpawnPosition(float size, float incrementAmount = 0.2f, int attempts = 20)
		{
			var position = new Vector3(size / 2, 50, size / 2);
			currentPlayer.transform.position = Vector3.zero;

			for (var x = 0; x < attempts; x++)
			{
				position.x += x * incrementAmount;
				position.z += x * incrementAmount;

				var hits = Physics.RaycastAll(position, Vector3.down, size);
				if (hits.Length == 0) continue;

				for (var i = 0; i < hits.Length; i++)
				{
					if (hits[i].collider.transform == currentPlayer.transform) continue;
					position.y = hits[i].point.y;
				
					
					
					var bounds = currentPlayer.GetComponentInChildren<SkinnedMeshRenderer>().bounds;
					if (!BoundDrawer.DetermineIfGeometryIsFlat(new BoundDrawer.GeometryFlatData(hits[i].point - new Vector3(0, bounds.extents.y, 0),
						    bounds, 1f,
						    groundLayer, Quaternion.identity))) continue;

					return position;
				}
			}


			Debug.Log("failed");

			return Vector3.positiveInfinity;
		}


		private void OnDisable() => MapGenerator.OnMapGenerated -= SpawnPlayer;
	}
}