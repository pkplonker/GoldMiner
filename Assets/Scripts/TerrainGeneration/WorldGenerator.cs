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
	public ChunkManager ChunkManager{ get; private set; }
	public event Action<int, int> OnChunkGeneratedAction;
	public event Action<int> MapGenerationStarted;
	public event Action MapGenerated;
	public event Action TerrainGenerated;

	[SerializeField]
	public MarchingCubeMapData MapData;

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
}