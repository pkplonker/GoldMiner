//
// Copyright (C) 2024 Stuart Heath. All rights reserved.
//

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///WorldGenerator full description
/// </summary>
public class WorldGenerator : MonoBehaviour, IService
{
	public ChunkManager ChunkManager { get; private set; }
	public event Action<int, int> OnChunkGeneratedAction;
	public event Action<int> MapGenerationStarted;
	public event Action MapGenerated;
	public event Action TerrainGenerated;

	[SerializeField]
	public MarchingCubeMapData MapData;

	[SerializeField]
	private float startDelayTimeForDebug = 0;

	private void Awake()
	{
		ServiceLocator.Instance.RegisterService<WorldGenerator>(this);
	}

	private void Start()
	{
		ChunkManager = ServiceLocator.Instance.GetService<ChunkManager>();
		ChunkManager.OnChunkGeneratedAction += OnChunkGenerated;
		ChunkManager.TerrainGenerated += OnMapGenerated;
		ChunkManager.TerrainGenerationStarted += OnMapGenerationStarted;
		StartCoroutine(StartDelayCor());
	}

	private IEnumerator StartDelayCor()
	{
		yield return new WaitForSeconds(startDelayTimeForDebug);
		Generate();
	}

	public void Generate()
	{
		ChunkManager.GenerateChunks(MapData);
	}

	private void OnChunkGenerated(int generatedCount, int requiredCount)
	{
		OnChunkGeneratedAction?.Invoke(generatedCount, requiredCount);
	}

	private void OnMapGenerated()
	{
		TerrainGenerated?.Invoke();
		MapGenerated?.Invoke();
	}

	private void OnMapGenerationStarted(int totalChunksRequired)
	{
		MapGenerationStarted?.Invoke(totalChunksRequired);
	}

	public void Initialize() { }

	public void ClearChunks()
	{
		ChunkManager.ClearChunks();
	}
}