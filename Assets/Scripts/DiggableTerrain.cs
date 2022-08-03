//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
///DiggableTerrain full description
/// </summary>
public class DiggableTerrain : MonoBehaviour
{
	private MeshCollider _meshCollider;
	private MeshFilter _meshFilter;
	private bool _setup = false;
	private Vector3[] _verts;


	public void Dig(RaycastHit hit, float digAmount = 0.1f)
	{
		if (!_setup) Setup();

		var mesh = _meshFilter.mesh;
		var hitVerts = GetHitVerts(hit, mesh);

		_verts ??= mesh.vertices;

		UpdateVerts(digAmount, hitVerts);
		var updatedMesh = RegenerateMesh(_verts);
		UpdateCollider(updatedMesh);
	}

	private void UpdateCollider(Mesh newMesh)
	{
		if (_meshCollider.sharedMesh) _meshCollider.sharedMesh.Clear();
		_meshCollider.sharedMesh = newMesh;
		_meshCollider.cookingOptions = MeshColliderCookingOptions.EnableMeshCleaning;
	}

	private Mesh RegenerateMesh(Vector3[] verts)
	{
		var oldMesh = _meshFilter.mesh;
		var newMesh = new Mesh
		{
			name = oldMesh.name
		};
		newMesh.SetVertices(verts);
		newMesh.triangles = oldMesh.GetTriangles(0);
		var uv = new List<Vector2>();
		oldMesh.GetUVs(0, uv);
		newMesh.SetUVs(0, uv);
		newMesh.SetNormals(oldMesh.normals);
		newMesh.RecalculateBounds();
		_meshFilter.mesh.SetVertices(verts);
		_meshFilter.mesh.RecalculateBounds();
		_meshFilter.mesh = newMesh;
		return newMesh;
	}

	private static Vector3[] GetHitVerts(RaycastHit hit, Mesh mesh)
	{
		var index = hit.triangleIndex;
		var hitVerts = new Vector3[3];
		for (var i = 0; i < hitVerts.Length; i++)
		{
			hitVerts[i] = (mesh.vertices[mesh.triangles[index * 3 + i]]);
		}

		return hitVerts;
	}

	private void UpdateVerts(float digAmount, Vector3[] hitVerts)
	{
		for (var i = 0; i < _verts.Length; i++)
		{
			var v = _verts[i];
			if (hitVerts.Any(hv => v == hv))
			{
				_verts[i] = new Vector3(v.x, v.y - digAmount, v.z);
			}
		}
	}

	private void Setup()
	{
		if (!_meshCollider) _meshCollider = GetComponent<MeshCollider>();
		if (!_meshFilter) _meshFilter = GetComponent<MeshFilter>();
		_setup = _meshCollider && _meshFilter;
	}
}