//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using TerrainGeneration;
using UnityEngine;

namespace Test
{
	/// <summary>
	///TerrainTest full description
	/// </summary>
	public class TerrainTest : MonoBehaviour
	{
		[SerializeField] private bool _spawnTerrainOnStart = true;

		private void Start()
		{
			if(_spawnTerrainOnStart) Spawn();
		}

		private void Spawn()
		{
			var mapGenerator = FindObjectOfType<MapGenerator>();
			mapGenerator.SpawnTerrain();
		}
	}
}