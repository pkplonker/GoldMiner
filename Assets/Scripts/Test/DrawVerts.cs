using System;
using UnityEditor;
using UnityEngine;

namespace Test
{
	public class DrawVerts : MonoBehaviour
	{
		private Mesh mesh;

		private void Awake()
		{
			GetMesh();
		}

		private void GetMesh()
		{
			var mf = GetComponent<MeshFilter>();
			if (!mf) return;
			if (mf.mesh)
			{
				mesh = mf.sharedMesh;
			}
		}


		private void OnDrawGizmosSelected()
		{
			#if UNITY_EDITOR
			if (mesh == null || mesh.vertices.Length==0) GetMesh();
			for (int i = 0; i < mesh.vertices.Length; i++)
			{
				Handles.color = Color.red;
				Handles.Label(transform.TransformPoint(mesh.vertices[i]), i.ToString());
			}
			#endif
		}
	}
}