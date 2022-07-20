using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.Profiling;
#endif

namespace Terrain
{
	[RequireComponent(typeof(MeshFilter))]
	public class TerrainGenerator : MonoBehaviour
	{
		[Range(2, 200)] [SerializeField] private int size;
		[Range(0.1f, 1f)] [SerializeField] private float vertexSpacing;

		private int detailLevel;
		private List<int> triangles = new List<int>();
		private List<Vector3> vertices = new List<Vector3>();
		private List<Vector2> uvs = new List<Vector2>();
		[SerializeField] private bool debug;
		private MeshCollider meshCollider;
		private MeshFilter meshFilter;
		[SerializeField] private Material debugMat;

		private void Awake()
		{
			detailLevel = Mathf.FloorToInt(1 / vertexSpacing);
			meshCollider = GetComponent<MeshCollider>();
			meshFilter = GetComponent<MeshFilter>();
			GenerateTerrain();
		}

		public void UpdateMesh(List<Vector3> verts)
		{
#if UNITY_EDITOR

			Profiler.BeginSample("Updating mesh");
#endif
			var mesh = GenerateMesh(verts, triangles, uvs);
			SetMesh(mesh, meshFilter);
			UpdateCollider(meshCollider, mesh);
#if UNITY_EDITOR

			Profiler.EndSample();
#endif
		}

		public void GenerateTerrain()
		{
#if UNITY_EDITOR

			Profiler.BeginSample("Generate Terrain");
#endif
			GenerateMeshData();
			var mesh = GenerateMesh(vertices, triangles, uvs);
			SetMesh(mesh, meshFilter);

			UpdateCollider(meshCollider, mesh);
#if UNITY_EDITOR

			Profiler.EndSample();
#endif
		}

		private void UpdateCollider(MeshCollider mc, Mesh mesh)
		{
			mc.sharedMesh = null;
			mc.sharedMesh = mesh;
		}

		private void SetMesh(Mesh generatedMesh, MeshFilter mf)
		{
			mf.mesh = generatedMesh;
		}

		private void GenerateMeshData()
		{
			var s = size * detailLevel;
			var topLeftX = (s - 1) / -2f;
			var topLeftZ = (s - 1) / 2f;
			for (var y = 0; y < s; y++)
			{
				for (var x = 0; x < s; x++)
				{
					vertices.Add(new Vector3((topLeftX + x) / detailLevel, 0, (topLeftZ - y) / detailLevel));
					uvs.Add(new Vector2(x / (float) s, y / (float) s));
					
					if (x >= s - 1 || y >= s - 1) continue;
					
					AddTriangles(s);
				}
			}
		}

		private void AddTriangles(int s)
		{
			var i = vertices.Count - 1;
			triangles.Add(i);
			triangles.Add(i + s + 1);
			triangles.Add(i + s);

			triangles.Add(i + s + 1);
			triangles.Add(i);
			triangles.Add(i + 1);
		}

		private Mesh GenerateMesh(List<Vector3> verts, List<int> tris, List<Vector2> u)
		{
			var mesh = new Mesh
			{
				name = "Terrain"
			};
			if (debug)
			{
				foreach (var v in vertices)
				{
					var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
					go.transform.localScale = Vector3.one * 0.1f;
					go.transform.SetParent(transform);
					go.transform.SetPositionAndRotation(v, Quaternion.identity);
					go.GetComponent<MeshRenderer>().sharedMaterial = debugMat;
				}
			}

			mesh.SetVertices(verts);
			mesh.SetTriangles(tris, 0);
			mesh.SetUVs(0, u);
			mesh.RecalculateNormals();
			return mesh;
		}
	}
}