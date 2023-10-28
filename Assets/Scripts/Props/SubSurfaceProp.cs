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
		[SerializeField] private float depthMinimum = 0.15f;
		[SerializeField] private float depthMaximum = 0.35f;
		public static float globalMaxDepth { get; private set; } = 0.5f;

		private void OnValidate()
		{
			if (depthMaximum > globalMaxDepth) depthMaximum = globalMaxDepth;
		}

		protected override bool CalculatePlacement(MapData mapData, List<Vector2> points, int i, float tolerance,
			out Vector3 result,
			out Quaternion rotation)
		{
			result = CalculatePosition(
				new Vector3(points[i].x + mapData.BoundaryInstep, 0, points[i].y + mapData.BoundaryInstep), mapData);
			rotation = CalculateRotation(i, mapData.seed);
			return result != Vector3.positiveInfinity;
		}

		protected override float GetDropIntoTerrainAmount(int seed, Vector3 position)
		{
			Random.InitState(seed + (int) position.x);
			return UtilityRandom.RandomRangeFloat(depthMinimum, depthMaximum);
		}
	}
}