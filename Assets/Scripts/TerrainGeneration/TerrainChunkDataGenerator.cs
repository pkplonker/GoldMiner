using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainGeneration
{
	public class TerrainChunkDataGenerator
	{
		private List<Vector3> verts;
		private List<int> triangles;
		private List<Vector2> uvs;
		private Vector3[] norms;


		public void Init(Action<TerrainChunkData> callback, int xCord, int yCord, MapData mapData, float[,] noiseMap)
		{
			var hc = new AnimationCurve(mapData.heightCurve.keys);

			var vertsPerRow = (mapData.MapChunkSize * mapData.lod) + 1;
			var colourMap =
				PopulateColorMapFromHeightMap(vertsPerRow, vertsPerRow, mapData.terrains, noiseMap, xCord, yCord);
			GenerateMeshData(vertsPerRow, mapData.lod, noiseMap, mapData.heightMultiplier, hc, xCord, yCord);
			callback?.Invoke(new TerrainChunkData(xCord, yCord, colourMap, verts, triangles, uvs, norms));
		}


		private void GenerateMeshData(int vertsPerRow, int lod, float[,] noiseMap, float heightMultiplier,
			AnimationCurve hc, int xCord, int yCord)
		{
			verts = new List<Vector3>(vertsPerRow * vertsPerRow);
			triangles = new List<int>(vertsPerRow * vertsPerRow * 6);
			uvs = new List<Vector2>(vertsPerRow * vertsPerRow);
			var vertMoveAmount = 1 / (float) lod;
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

			CalculateNormals(ref verts, out norms, ref triangles);
		}

		public static void CalculateNormals(ref List<Vector3> verts, out Vector3[] normals, ref List<int> triangles)
		{
			
			normals = new Vector3[verts.Count];
		
			for (var i = 0; i < triangles.Count / 3; i++)
			{
				var normalTriangleIndex = i * 3;
				var vertexIndexA = triangles[normalTriangleIndex];
				var vertexIndexB = triangles[normalTriangleIndex + 1];
				var vertexIndexC = triangles[normalTriangleIndex + 2];
		
				var triangleNormal = Vector3.up;
				normals[vertexIndexA] += triangleNormal;
				normals[vertexIndexB] += triangleNormal;
				normals[vertexIndexC] += triangleNormal;
			}
		
			for (var i = 0; i < normals.Length; i++)
			{
				//normals[i].Normalize();
			}
			
		}

		public static Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC, List<Vector3> verts) =>
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
						if (!(height <= t._height)) continue;
						colourMap[y * vertsPerRow + x] = t._color;
						break;
					}
				}
			}

			return colourMap;
		}
	}

	public struct TerrainChunkData
	{
		public readonly int X;
		public readonly int Y;
		public readonly Color[] ColourMap;
		public readonly List<Vector3> Verts;
		public readonly List<int> Triangles;
		public readonly List<Vector2> Uvs;
		public readonly Vector3[] Normals;

		public TerrainChunkData(int x, int y, Color[] colourMap, List<Vector3> verts, List<int> triangles,
			List<Vector2> uvs,
			Vector3[] normals)
		{
			X = x;
			Y = y;
			ColourMap = colourMap;
			Verts = verts;
			Triangles = triangles;
			Uvs = uvs;
			Normals = normals;
		}
	}
}