using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class ChangeTerrainHeight : MonoBehaviour
{
    private Terrain terrain;
    [SerializeField] private float height;
    private float[,] heights;
    private int resolution;
    private void Awake()
    {
        terrain = GetComponent<Terrain>();
        if (terrain == null) Debug.LogError("missing terrain");
        resolution = terrain.terrainData.heightmapResolution;

        SetHeight(0.01f,0,0,resolution,resolution);
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(30,30,200,30), "Change terrain Height"))
        {
            SetHeight(height,10,10,2,2);
        }
    }

    private  void SetHeight(float h, int xStart, int yStart, int xSize, int ySize)
    {
        Profiler.BeginSample("TerrainHeightChangeTest");
        Debug.Log("Res = " + resolution);
        heights = terrain.terrainData.GetHeights(0, 0, resolution, resolution);
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                heights[xStart+x, yStart+y] = h;
            }
        }

        terrain.terrainData.SetHeights(0, 0, heights);
        Profiler.EndSample();
    }
}
