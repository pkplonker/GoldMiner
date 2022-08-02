 //
 // Copyright (C) 2022 Stuart Heath. All rights reserved.
 //

 using System;
 using System.Collections;

 namespace TerrainGeneration
 {
	 /// <summary>
	 ///iProcessPropPointData full description
	 /// </summary>
    
	 public interface IProcessPropPointData
	 {
		 public IEnumerator ProcessPointDataCor(PoissonData poissonData, int currentAmount, int targetAmount,
			 Action<int, int> callback, PropSpawner propSpawner, MapData mapData);
	 }
 }
