using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.Profiling;
#endif

namespace Terrain
{
	public static class TerrainMeshGenerator
	{
		private static List<int> triangles = new List<int>();
		private static List<Vector3> vertices = new List<Vector3>();
		private static List<Vector2> uvs = new List<Vector2>();


		public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier,
			AnimationCurve heightCurve, int vertexCountMultiplier)
		{
			int width = heightMap.GetLength(0) ;
			int height = heightMap.GetLength(1) ;
			float topLeftX = (width/vertexCountMultiplier - 1) / -2f;
			float topLeftZ = (height/vertexCountMultiplier - 1) / 2f;
			MeshData meshData = new MeshData();
			int vertexIndex = 0;

			for (int y = 0; y < height; y ++) {
				for (int x = 0; x < width; x ++) {
					meshData.vertices.Add(new Vector3 (topLeftX + (x/(float)vertexCountMultiplier), heightCurve.Evaluate(heightMap [x, y]) * heightMultiplier, topLeftZ - (y/(float)vertexCountMultiplier))); ;
					meshData.uvs.Add(new Vector2 (x / (float)width, y / (float)height));

					if (x < width - 1 && y < height - 1) {
						meshData.AddTriangle (vertexIndex, vertexIndex + width + 1, vertexIndex + width);
						meshData.AddTriangle (vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
					}

					vertexIndex++;
				}
			}

			return meshData;
		}
	}

	public class MeshData
	{
		public List<Vector3> vertices;
		public List<int> triangles;
		public List<Vector2> uvs;


		public MeshData()
		{
			vertices = new List<Vector3>();
			uvs = new List<Vector2>();
			triangles = new List<int>();
		}

		public void AddTriangle(int a, int b, int c)
		{
			triangles.Add(a);
			triangles.Add(b);
			triangles.Add(c);
		}

		public Mesh CreateMesh()
		{
			Mesh mesh = new Mesh();
			mesh.SetVertices(vertices);
			mesh.SetTriangles(triangles, 0);
			mesh.SetUVs(0, uvs);
			mesh.RecalculateNormals();
			return mesh;
		}
	}
}