//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using TerrainGeneration;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Player
{
	/// <summary>
	///PlayerSpawner full description
	/// </summary>
	public class PlayerSpawner : SingleInstanceSpawn
	{
		public static event Action<GameObject> OnPlayerSpawned;
		public override void Setup(GameObject obj)
		{
			base.Setup(obj);
			OnPlayerSpawned?.Invoke(obj);
		}


	}
}