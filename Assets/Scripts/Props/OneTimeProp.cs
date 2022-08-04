 //
 // Copyright (C) 2022 Stuart Heath. All rights reserved.
 //

 using System;
 using System.Collections.Generic;
 using TerrainGeneration;
 using UnityEngine;

 namespace Props
 {
	 /// <summary>
	 ///OneTimeProp full description
	 /// </summary>
	 [CreateAssetMenu(fileName = "New One Time Prop",menuName = "Props/One Time Prop")]

	 public class OneTimeProp : Prop
	 {
		 protected override int CalculateNumberToSpawn(MapData mapData, List<Vector2> points) => 1;
	 }
 }
