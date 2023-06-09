//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System.Collections.Generic;
using StuartHeathTools;
using TerrainGeneration;
using UnityEngine;
using UnityEngine.Serialization;

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

		protected override bool CalculatePlacement(MapData mapData, List<Vector2> points, int i, float tolerance,
			out Vector3 result,
			out Quaternion rotation)
		{
			result = CalculatePosition(new Vector3(points[i].x, 0, points[i].y), mapData);
			rotation = CalculateRotation(i, mapData.seed);
			return result == Vector3.positiveInfinity;
		}

		protected override float GetDropIntoTerrainAmount(int seed, Vector3 position)
		{
			Random.InitState(seed + (int) position.x);
			return UtilityRandom.RandomRangeFloat(depthMinimum, depthMaximum);
		}
	}
}