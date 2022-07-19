using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class ChangeTerrainHeight : MonoBehaviour
{
	private UnityEngine.Terrain terrain;
	[SerializeField] private float startHeight=0.1f;
	[SerializeField] private float height;
	private float[,] heights;
	private int resolution;
	[SerializeField] private int holeSize = 20;

	private void Awake()
	{
		terrain = GetComponent<UnityEngine.Terrain>();
		if (terrain == null) Debug.LogError("missing terrain");
		resolution = terrain.terrainData.heightmapResolution;

		SetHeight(startHeight, 0, 0, resolution, resolution);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			SetHeight(startHeight-height, terrain.terrainData.heightmapResolution/2, terrain.terrainData.heightmapResolution/2, holeSize,holeSize);
		}
	}

	private void SetHeight(float h, int xStart, int yStart, int xSize, int ySize)
	{
		Profiler.BeginSample("TerrainHeightChangeTest");
		Debug.Log("Res = " + resolution);
		heights = terrain.terrainData.GetHeights(0, 0, resolution, resolution);
		for (int x = 0; x < xSize; x++)
		{
			for (int y = 0; y < ySize; y++)
			{
				heights[xStart + x, yStart + y] = h;
			}
		}

		terrain.terrainData.SetHeightsDelayLOD(0, 0, heights);
		Profiler.EndSample();
	}
}