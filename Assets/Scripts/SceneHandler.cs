using System;
using System.Collections;
using System.Collections.Generic;
using StuartHeathTools;
using TerrainGeneration;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour, IService
{
	private const string WorldGenScene = "WorldGenScene";

	private void Awake()
	{
		ServiceLocator.Instance.RegisterService(this);
	}

	public void HandleFailedGeneration(MarchingCubeMapData mapData)
	{
#if !UNITY_EDITOR
		var old = mapData.seed;
		mapData.seed *= 10;
		ServiceLocator.Instance.GetService<MapGenerator>().RegenerateWorld();
#endif
	}

	public void Initialize() { }

	public static void Load0()
	{
		SceneManager.LoadScene(0);
	}

	public static void Load1()
	{
		SceneManager.LoadScene(1);
	}
}