using System;
using System.Collections;
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
	private WaitForSeconds wait;
	private Coroutine cor;

	private void Awake()
	{
		mapGeneratorTerrain = GetComponent<MapGeneratorTerrain>();
		wait = new WaitForSeconds(refreshRate);
	}

	private void OnEnable() => MapGenerator.MapGenerated += MapGenerated;
	private void OnDisable() => MapGenerator.MapGenerated -= MapGenerated;

	private void MapGenerated(float notUsed)
	{
		if (cor != null)
		{
			StopCoroutine(cor);
		}

		StartCoroutine(CheckPlayerPosCor());
	}

	private IEnumerator CheckPlayerPosCor()
	{
		var size = MapGeneratorTerrain.terrainChunks.GetLength(0);
		if (playerReference.GetPlayer() == null)
		{
			Debug.Log("Player is null");
			yield break;
		}

		Transform playerTransform = playerReference.GetPlayer().transform;

		while (true)
		{
			try
			{
				for (var x = 0; x < size; x++)
				{
					for (var y = 0; y < size; y++)
					{
						var chunkCenter = MapGeneratorTerrain.terrainChunks[x, y].transform.position;
						float distanceToChunk = Vector3.Distance(playerTransform.position, chunkCenter);

						if (distanceToChunk <= viewDistance)
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
				catch {Debug.LogWarning("Failed to get player"); }
			}

			yield return wait;
		}
	}
}