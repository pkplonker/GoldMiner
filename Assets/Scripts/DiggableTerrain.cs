//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using TerrainGeneration;
using UnityEngine;

/// <summary>
///DiggableTerrain full description
/// </summary>
public class DiggableTerrain : MonoBehaviour
{
	private MeshCollider _meshCollider;
	private MeshFilter _meshFilter;
	private bool _setup = false;
	private List<DiggableTerrain> neighbours = new List<DiggableTerrain>();
	private float _digAmount;

	public void Dig(RaycastHit hit, float digAmount = 0.1f)
	{
		if (!_setup) Setup();
		_digAmount = digAmount;
		var mesh = _meshFilter.mesh;
		var hitVerts = GetHitVerts(hit, mesh);

		
		var verts =UpdateVerts(digAmount, hitVerts,mesh.vertices);
		var updatedMesh = RegenerateMesh(verts);
		UpdateCollider(updatedMesh);
		CheckNeighbours(hit, updatedMesh);
	}

	private void CheckNeighbours(RaycastHit hit, Mesh mesh)
	{
		List<Tuple<int, int, int>> changes = new List<Tuple<int, int, int>>();
		var terrainChunk = GetComponent<TerrainChunk>();
		var mapdata = terrainChunk.MapData;
		var x = terrainChunk.X;
		var y = terrainChunk.Y;
		var hitVertsIndexes = new int[3];
		var triangleIndex = hit.triangleIndex;

		for (var i = 0; i < hitVertsIndexes.Length; i++)
		{
			hitVertsIndexes[i] = mesh.triangles[triangleIndex * 3 + i];
		}

		var vertsPerRow = (mapdata.MapChunkSize * mapdata._lod) + 1;
		var totalVerts = vertsPerRow * vertsPerRow;


		foreach (var index in hitVertsIndexes)
		{
			var val = CheckTop(index, vertsPerRow, totalVerts);
			if (val != -1)
			{
				changes.Add(new Tuple<int, int, int>(x, y + 1, val));
			}

			val = CheckBottom(index, vertsPerRow, totalVerts);
			if (val != -1)
			{
				changes.Add(new Tuple<int, int, int>(x, y - 1, val));
			}


			val = CheckLeft(index, vertsPerRow);
			if (val != -1)
			{
				changes.Add(new Tuple<int, int, int>(x - 1, y, val));
			}

			val = CheckRight(index, vertsPerRow);
			if (val != -1)
			{
				changes.Add(new Tuple<int, int, int>(x + 1, y, val));
			}

			if (index == 0) //0
			{
				changes.Add(new Tuple<int, int, int>(x - 1, y - 1, totalVerts - 1)); //15
			}

			if (index == totalVerts - 1) //15
			{
				changes.Add(new Tuple<int, int, int>(x + 1, y + 1, 0)); //0
			}

			if (index == vertsPerRow - 1) //3
			{
				changes.Add(new Tuple<int, int, int>(x + 1, y - 1, totalVerts - vertsPerRow)); //12
			}

			if (index == totalVerts - vertsPerRow) //12
			{
				changes.Add(new Tuple<int, int, int>(x - 1, y + 1, vertsPerRow - 1)); //3
			}
		}

		ProcessNeighbourChanges(changes, mesh);
	}

	private void ProcessNeighbourChanges(List<Tuple<int, int, int>> changesRequested, Mesh mesh)
	{
		var changes = changesRequested.Distinct().ToList();
		if (changes.Count != changesRequested.Count)
		{
			Debug.Log("f");
		}

		foreach (var change in changes)
		{
			if (change.Item1 < 0 || change.Item1 > MapGeneratorTerrain.terrainChunks.GetLength(0)) continue;
			if (change.Item2 < 0 || change.Item2 > MapGeneratorTerrain.terrainChunks.GetLength(1)) continue;
			try
			{
				MapGeneratorTerrain.terrainChunks[change.Item1, change.Item2].GetComponent<DiggableTerrain>()
					.DigAtVertIndex(change.Item3, _digAmount);
			}catch(Exception e)
			{
				Debug.Log(e);
			}
		
			Debug.Log(change.Item1 + " " + change.Item2 + " " + change.Item3);
		}
	}

	private void DigAtVertIndex(int index, float digAmount)
	{
		if (digAmount == 0)
		{
			throw new Exception("dig depth is 0");
		}
		_digAmount = digAmount;
		if (!_setup) Setup();
		var verts = _meshFilter.mesh.vertices;
		verts[index].y -= digAmount;
		var updatedMesh = RegenerateMesh(verts);
		UpdateCollider(updatedMesh);
	}


	private int CheckLeft(int index, int vertsPerRow)
	{
		if (index % vertsPerRow == 0)
		{
			return index + vertsPerRow - 1;
		}

		return -1;
	}

	private int CheckRight(int index, int vertsPerRow)
	{
		if ((index + 1) % vertsPerRow == 0)
		{
			return index - vertsPerRow + 1;
		}


		return -1;
	}

	private int CheckBottom(int index, int vertsPerRow, int totalVerts)
	{
		if (index < vertsPerRow)
		{
			return totalVerts - vertsPerRow + index;
		}

		return -1;
	}

	private int CheckTop(int index, int vertsPerRow, int totalVerts)
	{
		if (index >= totalVerts - vertsPerRow)
		{
			return index % vertsPerRow;
		}

		return -1;
	}

	private Vector3[] UpdateVerts(float digAmount, Vector3[] hitVerts,Vector3[] verts)
	{
		for (var i = 0; i < verts.Length; i++)
		{
			var v = verts[i];
			if (hitVerts.Any(hv => v == hv))
			{
				verts[i] = new Vector3(v.x, v.y - digAmount, v.z);
			}
		}

		return verts;
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
		var vertList = verts.ToList();
		newMesh.SetVertices(verts);
		newMesh.triangles = oldMesh.GetTriangles(0);
		var oldUvs = new List<Vector2>();
		oldMesh.GetUVs(0, oldUvs);
		newMesh.SetUVs(0, oldUvs);
		//newMesh.SetNormals(oldMesh.normals);
		var normals = new Vector3[verts.Length];
		var tris = newMesh.triangles.ToList();
		TerrainChunkDataGenerator.CalculateNormals(ref vertList, out normals,ref  tris);
		newMesh.SetNormals(normals);
		newMesh.RecalculateBounds();
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


	private void Setup()
	{
		if (!_meshCollider) _meshCollider = GetComponent<MeshCollider>();
		if (!_meshFilter) _meshFilter = GetComponent<MeshFilter>();
		_setup = _meshCollider && _meshFilter;
	}
}