using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.Profiling;
#endif

namespace Terrain
{
	[RequireComponent(typeof(MeshFilter))]
	public class DiggableTerrain : MonoBehaviour
	{
		private MeshCollider meshCollider;
		private MeshFilter meshFilter;

		private void Awake()
		{
			meshCollider = GetComponent<MeshCollider>();
			meshFilter = GetComponent<MeshFilter>();
		}

		public void UpdateMesh(List<Vector3> verts)
		{
#if UNITY_EDITOR

			Profiler.BeginSample("Updating mesh");
#endif
			Mesh mesh = meshFilter.mesh;

			mesh.SetVertices(verts);
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
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

	
	}
}