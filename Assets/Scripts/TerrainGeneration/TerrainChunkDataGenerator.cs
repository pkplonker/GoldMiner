using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TerrainGeneration
{
	public class TerrainChunkDataGenerator
	{
		private List<Vector3> verts;
		private List<int> triangles;
		private List<Vector2> uvs;
		Vector3[] norms;


		public void Init(Action<TerrainChunkData> callback, int xCord, int yCord, MapData mapData, float[,] noiseMap)
		{
			var hc = new AnimationCurve(mapData.heightCurve.keys);

			var vertsPerRow = (mapData.mapChunkSize * mapData.LOD) + 1;
			var colourMap =
				PopulateColorMapFromHeightMap(vertsPerRow, vertsPerRow, mapData.terrains, noiseMap, xCord, yCord);
			GenerateMeshData(vertsPerRow, mapData.LOD, noiseMap, mapData.heightMultiplier, hc, xCord, yCord);
			callback?.Invoke(new TerrainChunkData(xCord, yCord, colourMap, verts, triangles, uvs, norms));
		}


		private void GenerateMeshData(int vertsPerRow, int LOD, float[,] noiseMap, float heightMultiplier,
			AnimationCurve hc, int xCord, int yCord)
		{
			verts = new List<Vector3>(vertsPerRow * vertsPerRow);
			triangles = new List<int>(vertsPerRow * vertsPerRow * 6);
			uvs = new List<Vector2>(vertsPerRow * vertsPerRow);
			var vertMoveAmount = 1 / (float) LOD;
			for (var y = 0; y < vertsPerRow; y++)
			{
				for (var x = 0; x < vertsPerRow; x++)
				{
					verts.Add(new Vector3(x * vertMoveAmount,
						hc.Evaluate(noiseMap[(xCord * vertsPerRow) + x - xCord,
							(yCord * vertsPerRow) + y - yCord]) * heightMultiplier, y * vertMoveAmount));
					uvs.Add(new Vector2(x / (float) vertsPerRow, y / (float) vertsPerRow));
					var currentVertIndex = verts.Count - 1;
					if (x >= vertsPerRow - 1 || y >= vertsPerRow - 1) continue;
					//triangle 1
					triangles.Add(currentVertIndex);
					triangles.Add(currentVertIndex + vertsPerRow);
					triangles.Add(currentVertIndex + vertsPerRow + 1);
					//triangle 2
					triangles.Add(currentVertIndex + vertsPerRow + 1);
					triangles.Add(currentVertIndex + 1);
					triangles.Add(currentVertIndex);
				}
			}

			CalculateNormals();
		}

		private void CalculateNormals()
		{
			norms = new Vector3[verts.Count];

			Vector3 triangleNormal;
			for (var i = 0; i < triangles.Count / 3; i++)
			{
				var normalTriangleIndex = i * 3;
				var vertexIndexA = triangles[normalTriangleIndex];
				var vertexIndexB = triangles[normalTriangleIndex + 1];
				var vertexIndexC = triangles[normalTriangleIndex + 2];

				triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
				norms[vertexIndexA] += triangleNormal;
				norms[vertexIndexB] += triangleNormal;
				norms[vertexIndexC] += triangleNormal;
			}

			for (var i = 0; i < norms.Length; i++)
			{
				norms[i].Normalize();
			}
		}

		Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC) =>
			Vector3.Cross(verts[indexB] - verts[indexA], verts[indexC] - verts[indexA]).normalized;


		private Color[] PopulateColorMapFromHeightMap(int mapSize, int vertsPerRow, Terrains[] terrains,
			float[,] noiseMap,
			int xCord, int yCord)
		{
			var colourMap = new Color[mapSize * mapSize];
			for (var y = 0; y < vertsPerRow; y++)
			{
				for (var x = 0; x < vertsPerRow; x++)
				{
					var height = noiseMap[(xCord * vertsPerRow) + x, (yCord * vertsPerRow) + y];
					foreach (var t in terrains)
					{
						if (!(height <= t.height)) continue;
						colourMap[y * vertsPerRow + x] = t.color;
						break;
					}
				}
			}

			return colourMap;
		}
	}

	public struct TerrainChunkData
	{
		public readonly int x;
		public readonly int y;
		public readonly Color[] colourMap;
		public readonly List<Vector3> verts;
		public readonly List<int> triangles;
		public readonly List<Vector2> uvs;
		public readonly Vector3[] normals;

		public TerrainChunkData(int x, int y, Color[] colourMap, List<Vector3> verts, List<int> triangles,
			List<Vector2> uvs,
			Vector3[] normals)
		{
			this.x = x;
			this.y = y;
			this.colourMap = colourMap;
			this.verts = verts;
			this.triangles = triangles;
			this.uvs = uvs;
			this.normals = normals;
		}
	}
}