//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using System.Collections;
using TerrainGeneration;
using UnityEngine;

namespace Player
{
	/// <summary>
	///PlayerSpawner full description
	/// </summary>
	public class PlayerSpawner : MonoBehaviour
	{
		[SerializeField] private GameObject _playerPrefab;
		[SerializeField] private Vector3 _offset;

		[SerializeField] private string _groundLayer = "Ground";
		private GameObject _currentPlayer;

		private void OnEnable()
		{
			MapGenerator.OnMapGenerated += SpawnPlayer;
			MapGenerator.OnMapGenerationStarted += DespawnPlayer;
		}

		private void DespawnPlayer(int notRequired = 0, int notRequired2 = 0)
		{
			if (_currentPlayer )
			{
				Destroy(_currentPlayer);
			}
		}


		private void SpawnPlayer(float size)
		{
			DespawnPlayer();

			_currentPlayer = Instantiate(_playerPrefab);
			_currentPlayer.SetActive(false);
			var spawnPos = CalculateSpawnPosition(size) + _offset;

			if (spawnPos != Vector3.positiveInfinity)
			{
				_currentPlayer.transform.position = spawnPos;
				_currentPlayer.GetComponent<PlayerMovement>().SetCanMove(true);
				_currentPlayer.SetActive(true);
			}
			else HandleFailedSpawn();
		}

		private void HandleFailedSpawn()
		{
			_currentPlayer.GetComponent<PlayerMovement>().SetCanMove(false);

			Debug.LogError("Failed to spawn player");
			throw new NotImplementedException();
		}


		private Vector3 CalculateSpawnPosition(float size, float incrementAmount = 0.2f, int attempts = 20)
		{
			var position = new Vector3(size / 2, 50, size / 2);
			_currentPlayer.transform.position = Vector3.zero;

			for (var x = 0; x < attempts; x++)
			{
				position.x += x * incrementAmount;
				position.z += x * incrementAmount;

				var hits = Physics.RaycastAll(position, Vector3.down, size);
				if (hits.Length == 0) continue;

				for (var i = 0; i < hits.Length; i++)
				{
					if (hits[i].collider.transform == _currentPlayer.transform) continue;
					position.y = hits[i].point.y;


					var bounds = _currentPlayer.GetComponentInChildren<SkinnedMeshRenderer>().bounds;
					if (!BoundDrawer.DetermineIfGeometryIsFlat(new BoundDrawer.GeometryFlatData(
						    hits[i].point - new Vector3(0, bounds.extents.y, 0),
						    bounds, 1f,
						    _groundLayer, Quaternion.identity))) continue;

					return position;
				}
			}


			Debug.Log("failed");

			return Vector3.positiveInfinity;
		}


		private void OnDisable() => MapGenerator.OnMapGenerated -= SpawnPlayer;
	}
}