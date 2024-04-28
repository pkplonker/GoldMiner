using System;
using System.Collections;
using System.Collections.Generic;
using StuartHeathTools;
using TerrainGeneration;
using UnityEngine;
using UnityEngine.Serialization;

namespace Props
{
	[Serializable]
	public abstract class Prop : ScriptableObject
	{
		[SerializeField] public bool StaticObject = true;
		[SerializeField] public bool Spawn = true;

		[SerializeField] public GameObject Prefab;

		[Range(-1f, 1f), SerializeField] public float FlatnessTolerance = 0.1f;

		[Range(0.1f, 10f), SerializeField] public float Radius;

		[SerializeField] public bool OverrideRadius = false;
		[SerializeField] public bool InBounderyOnly = false;
		
	}
}