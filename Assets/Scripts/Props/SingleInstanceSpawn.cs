//
// Copyright (C) 2023 Stuart Heath. All rights reserved.
//

using TerrainGeneration;
using UnityEngine;

/// <summary>
///SingleInstanceSpawn full description
/// </summary>
[CreateAssetMenu(fileName = "Single instance spawn", menuName = "Props/Spawns/Single instance spawn")]

public abstract class SingleInstanceSpawn : ScriptableObject
{
	[field: SerializeField] public bool allowFailure { get; private set; } = false;
	public abstract bool Spawn(MapData mapData, out GameObject gameObject);
	public abstract string GetName();
}