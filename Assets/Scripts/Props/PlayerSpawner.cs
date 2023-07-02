//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using UnityEngine;

namespace Props
{
	/// <summary>
	///PlayerSpawner full description
	/// </summary>
	[CreateAssetMenu(fileName = "New player instance spawn", menuName = "Props/Spawns/New player instance spawn")]

	public class PlayerSpawner : MultiAttemptSingleInstanceSpawn
	{
		public static event Action<GameObject> OnPlayerSpawned;
		protected override void Setup(GameObject obj)
		{
			base.Setup(obj);
			OnPlayerSpawned?.Invoke(obj);
		}


	}
}