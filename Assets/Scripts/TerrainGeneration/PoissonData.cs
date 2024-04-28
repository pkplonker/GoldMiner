using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace TerrainGeneration
{
	public struct PoissonData
	{
		public readonly int Index;
		public readonly List<Vector2> Points;

		public PoissonData(int index, List<Vector2> points)
		{
			Index = index;
			Points = points;
		}
	}
}