using System.Collections;
using System.Collections.Generic;
using StuartHeathTools;
using TerrainGeneration;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : GenericUnitySingleton<SceneHandler>
{
	private const string WorldGenScene = "WorldGenScene";

	public void HandleFailedGeneration(MapData mapData)
	{
		var old = mapData.seed;
		mapData.seed *= 10;
		Debug.LogWarning($"Failed creating scene{old}, changing to {mapData.seed}");
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}