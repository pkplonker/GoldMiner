using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class ChangeTerrainHeight : MonoBehaviour
{
	private Terrain _terrain;
	[SerializeField] private float _startHeight=0.1f;
	[SerializeField] private float _height;
	private float[,] _heights;
	private int _resolution;
	[SerializeField] private int _holeSize = 20;

	private void Awake()
	{
		_terrain = GetComponent<UnityEngine.Terrain>();
		if (_terrain == null) Debug.LogError("missing terrain");
		_resolution = _terrain.terrainData.heightmapResolution;

		SetHeight(_startHeight, 0, 0, _resolution, _resolution);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			SetHeight(_startHeight-_height, _terrain.terrainData.heightmapResolution/2, _terrain.terrainData.heightmapResolution/2, _holeSize,_holeSize);
		}
	}

	private void SetHeight(float h, int xStart, int yStart, int xSize, int ySize)
	{
		Profiler.BeginSample("TerrainHeightChangeTest");
		Debug.Log("Res = " + _resolution);
		_heights = _terrain.terrainData.GetHeights(0, 0, _resolution, _resolution);
		for (int x = 0; x < xSize; x++)
		{
			for (int y = 0; y < ySize; y++)
			{
				_heights[xStart + x, yStart + y] = h;
			}
		}

		_terrain.terrainData.SetHeightsDelayLOD(0, 0, _heights);
		Profiler.EndSample();
	}
}