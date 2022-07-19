using System;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{
	[RequireComponent(typeof(MeshFilter))]
	public class TerrainGenerator : MonoBehaviour
	{
		[SerializeField] private int size;
		private List<int> triangles = new List<int>();
		private List<Vector3> vertices = new List<Vector3>();
		[SerializeField] private bool debug;
		private MeshCollider meshCollider;
		private MeshFilter meshFilter;

		private void Awake()
		{
			meshCollider = GetComponent<MeshCollider>();
			meshFilter = GetComponent<MeshFilter>();
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				GenerateTerrain();
			}
		}

		public void GenerateTerrain()
		{
			GenerateMeshData();
			var mesh = GenerateMesh();
			SetMesh(mesh, meshFilter);

			UpdateCollider(meshCollider, mesh);
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
			for (int y = 0; y < size; y++)
			{
				for (int x = 0; x < size; x++)
				{
					vertices.Add(new Vector3(x, 0, y));
					if (x < size - 1 && y < size - 1)
					{
						var i = vertices.Count - 1;
						triangles.Add(i);
						triangles.Add(i + size);
						triangles.Add(i + size + 1);

						triangles.Add(i + size + 1);
						triangles.Add(i + 1);
						triangles.Add(i);
					}
				}
			}
		}

		private Mesh GenerateMesh()
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
				}
			}

			mesh.SetVertices(vertices);
			mesh.SetTriangles(triangles, 0);
			mesh.RecalculateNormals();
			return mesh;
		}
	}
}