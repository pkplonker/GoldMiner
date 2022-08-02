using System;
using System.Collections;
using System.Collections.Generic;
using StuartHeathTools;
using UnityEngine;

namespace TerrainGeneration
{
	[Serializable]
	public class PropData
	{
		[field: SerializeField] public List<Prop> Props;
	}
}