 //
 // Copyright (C) 2022 Stuart Heath. All rights reserved.
 //

 using System.Collections.Generic;
 using TerrainGeneration;
 using UnityEngine;

    /// <summary>
    ///PropTargetData full description
    /// </summary>
    [CreateAssetMenu(fileName = "Sub Surface Prop", menuName = "Props/Sub Surface Prop")]

public class SubSurfaceProp : Prop
    {
	    [SerializeField] private float _depthMinimum;
	    [SerializeField] private float _depthMaximum;

	protected override bool CalculatePlacement(MapData mapData, List<Vector2> points, int i, float tolerance,
		out Vector3 result,
		out Quaternion rotation)
	{
		result = CalculatePosition(new Vector3(points[i].x, 0, points[i].y),
			mapData);
		rotation = CalculateRotation(i, mapData._seed);

		if (result == Vector3.positiveInfinity) return true;
		var bounds = BoundDrawer.GetBounds(Prefab);
		if (!BoundDrawer.DetermineIfGeometryIsFlat(new BoundDrawer.GeometryFlatData(
			    result - new Vector3(0, bounds.extents.y, 0),
			    bounds, tolerance, mapData._terrainLayer, rotation))) return true;
		return false;
	}

	protected override float GetDropIntoTerrainAmount() => Random.Range(_depthMinimum, _depthMaximum);
}
