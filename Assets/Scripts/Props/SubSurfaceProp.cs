//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using System.Collections.Generic;
using StuartHeathTools;
using TerrainGeneration;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Props
{
	/// <summary>
	///PropTargetData full description
	/// </summary>
	[CreateAssetMenu(fileName = "Sub Surface Prop", menuName = "Props/Sub Surface Prop")]
	public class SubSurfaceProp : Prop
	{
		[SerializeField]
		private float DepthMinimum = 0.15f;

		[SerializeField]
		private float DepthMaximum = 0.35f;

		public static float GlobalMaxDepth { get; private set; } = 0.5f;

		private void OnValidate()
		{
			if (DepthMaximum > GlobalMaxDepth) DepthMaximum = GlobalMaxDepth;
		}
	}
}