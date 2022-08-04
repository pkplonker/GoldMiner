 //
 // Copyright (C) 2022 Stuart Heath. All rights reserved.
 //

 using System.Collections.Generic;
 using TerrainGeneration;
 using UnityEngine;

 namespace Props
 {
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

			 return result == Vector3.positiveInfinity;
		 }
	
		 protected override Vector3 CalculatePosition(Vector3 position, MapData mapData, float factor = 10)
		 {
			 position.y = mapData._heightMultiplier;

			 if (!Physics.Raycast(position, Vector3.down, out var hit, mapData._heightMultiplier + factor,
				     LayerMask.GetMask(mapData._terrainLayer))) return Vector3.positiveInfinity;

			 position.y = hit.point.y - GetDropIntoTerrainAmount();
			 return position;
		 }

		 protected override float GetDropIntoTerrainAmount() => Random.Range(_depthMinimum, _depthMaximum);
	 }
 }
