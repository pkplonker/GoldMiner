using System;
using System.Collections.Generic;
using System.Linq;
using Player;
using TerrainGeneration;
using UnityEngine;

public class ChunkBalancer : MonoBehaviour
{
	[SerializeField] private int viewDistance = 50;
	private MapGeneratorTerrain mapGeneratorTerrain;
	[SerializeField] private PlayerReference playerReference;
	[SerializeField] private float refreshRate = 1f;
	[SerializeField] private float fovAngle = 90f;
	[SerializeField] private float closeThreshold = 35f;

	private float timeSinceLastCheck = 0f;
	private Transform playerTransform;

	private void Awake()
	{
		mapGeneratorTerrain = GetComponent<MapGeneratorTerrain>();
	}

	private void OnEnable() => ServiceLocator.Instance.GetService<MapGenerator>().MapGenerated += MapGenerated;
	private void OnDisable() => ServiceLocator.Instance.GetService<MapGenerator>().MapGenerated -= MapGenerated;

	private void MapGenerated(float notUsed)
	{
		CheckPlayerPos();
	}

	private void Update()
	{
		timeSinceLastCheck += Time.deltaTime;
		if (timeSinceLastCheck >= refreshRate)
		{
			CheckPlayerPos();
			timeSinceLastCheck = 0f;
		}
	}

	private void CheckPlayerPos()
	{
		var size = MapGeneratorTerrain.terrainChunks.GetLength(0);
		if (playerReference.GetPlayer() == null)
		{
			Debug.Log("Player is null");
			return;
		}

		playerTransform = playerReference.GetPlayer().transform;
		try
		{
			for (var x = 0; x < size; x++)
			{
				for (var y = 0; y < size; y++)
				{
					var chunkCenter = MapGeneratorTerrain.terrainChunks[x, y].transform.position;
					float distanceToChunk = Vector3.Distance(playerTransform.position, chunkCenter);

					Vector3 directionToChunk = (chunkCenter - playerTransform.position).normalized;
					float angleToChunk = Vector3.Angle(playerTransform.forward, directionToChunk);

					bool isInFOV = angleToChunk < fovAngle;

					if (distanceToChunk <= closeThreshold || (distanceToChunk <= viewDistance && isInFOV))
					{
						MapGeneratorTerrain.terrainChunks[x, y].SetActive(true);
					}
					else
					{
						MapGeneratorTerrain.terrainChunks[x, y].SetActive(false);
					}
				}
			}
		}
		catch
		{
			try
			{
				playerTransform = playerReference.GetPlayer().transform;
			}
			catch
			{
				Debug.LogWarning("Failed to get player");
			}
		}
	}
}