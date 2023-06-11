using System;
using System.Collections;
using System.Collections.Generic;
using Props;
using StuartHeathTools;
using UnityEngine;

namespace TerrainGeneration
{
	[CreateAssetMenu(fileName = "Prop Data", menuName = "Props/Prop Data")]
	public class PropCollection : ScriptableObject
	{
		[field: SerializeField] public List<Prop> Props;
	}
}