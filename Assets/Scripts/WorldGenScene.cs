//
// Copyright (C) 2023 Stuart Heath. All rights reserved.
//

using System;
using TerrainGeneration;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
///WorldGenScene full description
/// </summary>
public class WorldGenScene : MonoBehaviour
{
	[SerializeField] private MapData mapData;
	[SerializeField] private TMP_InputField inputField;
	[SerializeField] private int seedMax;
	[SerializeField] private int seedMin;
	[SerializeField] private string mainSceneName = "World";

	private void OnValidate()
	{
		if (mapData == null) Debug.LogError("Missing mapdata");
		if (inputField == null) Debug.LogError("Missing inputField");
	}

	private void Awake() => UpdateInputToSeed();

	private void UpdateInputToSeed() => inputField.text = mapData.seed.ToString();

	// UI button
	public void RandomBtn()
	{
		mapData.seed = UnityEngine.Random.Range(seedMin, seedMax);
		UpdateInputToSeed();
	}

	// UI button
	public void StartBtn()
	{
		SceneManager.LoadScene(mainSceneName);
	}
}