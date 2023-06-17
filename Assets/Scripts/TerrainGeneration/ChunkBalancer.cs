using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Player;
using TerrainGeneration;
using UnityEngine;

public class ChunkBalancer : MonoBehaviour
{
    [SerializeField] private int viewDistance =50;
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
    private void OnDisable() => MapGenerator.MapGenerated -=  MapGenerated;

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
        var d = (viewDistance / mapGeneratorTerrain.MapData.MapChunkSize)/2;
        if (d < 1) d = 1;
        var size = MapGeneratorTerrain.terrainChunks.GetLength(0);
        while (playerReference.GetPlayer()!=null)
        {
            var index = mapGeneratorTerrain.GetChunkIndexFromPosition(playerReference.GetPlayer().transform.position);
            for (var x = 0; x < size; x++)
            {
                for (var y = 0; y < size; y++)
                {
                    if ((x >= index.x - d && x <= index.x + d)&&(y >= index.y - d && y <= index.y + d))
                    {
                        MapGeneratorTerrain.terrainChunks[x, y].gameObject.SetActive(true);
                    }
                    else
                    {
                        MapGeneratorTerrain.terrainChunks[x, y].gameObject.SetActive(false);
                    }
                }
            }
            yield return wait;
        }
        Debug.Log("Player is null");
    }
}
