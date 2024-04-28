//
// Copyright (C) 2024 Stuart Heath. All rights reserved.
//

using System;
using UnityEngine;

/// <summary>
///WorldGenerator full description
/// </summary>
public class WorldGenerator : MonoBehaviour, IService
{
	public event Action<int, int> OnChunkGeneratedAction;
	public event Action<int> MapGenerationStarted;
	public event Action MapGenerated;

	private void Awake()
	{
		ServiceLocator.Instance.RegisterService<WorldGenerator>(this);
	}

	private void Start()
	{
		var chunkManager = ServiceLocator.Instance.GetService<ChunkManager>();
		chunkManager.OnChunkGeneratedAction += OnChunkGenerated;
		chunkManager.MapGenerated += OnMapGenerated;
		chunkManager.MapGenerationStarted += OnMapGenerationStarted;
		chunkManager.GenerateChunks();
	}

	private void OnChunkGenerated(int generatedCount, int requiredCount)
	{
		OnChunkGeneratedAction?.Invoke(generatedCount, requiredCount);
	}

	private void OnMapGenerated()
	{
		MapGenerated?.Invoke();
	}

	private void OnMapGenerationStarted(int totalChunksRequired)
	{
		MapGenerationStarted?.Invoke(totalChunksRequired);
	}

	public void Initialize() { }
}