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
	private Vector3[] _verts;
	private List<DiggableTerrain> neighbours = new List<DiggableTerrain>();
	private float _digAmount;

	public void Dig(RaycastHit hit, float digAmount = 0.1f)
	{
		if (!_setup) Setup();
		_digAmount = digAmount;
		var mesh = _meshFilter.mesh;
		var hitVerts = GetHitVerts(hit, mesh);

		_verts ??= mesh.vertices;
		CheckNeighbours(hit, mesh);
		UpdateVerts(digAmount, hitVerts);
		var updatedMesh = RegenerateMesh(_verts);
		UpdateCollider(updatedMesh);
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

		var vertsPerRow = mapdata.MapChunkSize * mapdata._lod;
		var totalVerts = ((mapdata.MapChunkSize * mapdata._lod + 1) * (mapdata.MapChunkSize * mapdata._lod + 1)) - 1;
		

		foreach (var index in hitVertsIndexes)
		{
			var val = CheckTop(index, vertsPerRow, totalVerts);
			if (val != -1)
			{
				changes.Add( new Tuple<int,int,int>(x,y+1,val));
			}

			val = CheckBottom(index, vertsPerRow, totalVerts);
			if (val != -1)
			{
				changes.Add( new Tuple<int,int,int>(x,y-1,val));
			}

			val = CheckRight(index, vertsPerRow, totalVerts);
			if (val != -1)
			{
				changes.Add( new Tuple<int,int,int>(x-1,y,val));
			}

			val = CheckLeft(index, vertsPerRow, totalVerts);
			if (val != -1)
			{
				changes.Add( new Tuple<int,int,int>(x+1,y,val));
			}
		}

		ProcessNeighbourChanges(changes);
	}

	private void ProcessNeighbourChanges(List<Tuple<int,int, int>> changes)
	{
		changes = changes.Distinct().ToList();
		foreach (var change in changes)
		{
			if(change.Item1<0 || change.Item1>=MapGeneratorTerrain.terrainChunks.Length) continue;
			if(change.Item2<0 || change.Item2>=MapGeneratorTerrain.terrainChunks.Length) continue;
			MapGeneratorTerrain.terrainChunks[change.Item2,change.Item1].GetComponent<DiggableTerrain>().DigAtVertIndex(change.Item3,_digAmount);
		}
	}

	private void DigAtVertIndex(int index, float digamount)
	{
		_digAmount = digamount;
		if (!_setup) Setup();
		var verts = _meshFilter.mesh.vertices;
		verts[index].y -= _digAmount;
		var updatedMesh = RegenerateMesh(verts);
		UpdateCollider(updatedMesh);
	}

	
	private int CheckLeft(int index, int vertsPerRow, int totalVerts)
	{
		
		if (index % vertsPerRow == 0)
		{
			
			return index - vertsPerRow + 1;
		}

		return -1;
	}

	private int CheckRight(int index, int vertsPerRow, int totalVerts)
	{
	
		if ((index + 1) % vertsPerRow == 0)
		{
			return index + vertsPerRow - 1;
		}

		return -1;
	}

	private int CheckBottom(int index, int vertsPerRow, int totalVerts)
	{
		
		if (index < vertsPerRow)
		{
			return totalVerts - index-1;
		}

		return -1;
	}

	private int CheckTop(int index, int vertsPerRow, int totalVerts)
	{
		if (index > (((vertsPerRow - 1) * vertsPerRow) - 1))
		{
			return index % vertsPerRow;
		}

		return -1;
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


	private void Setup()
	{
		if (!_meshCollider) _meshCollider = GetComponent<MeshCollider>();
		if (!_meshFilter) _meshFilter = GetComponent<MeshFilter>();
		_setup = _meshCollider && _meshFilter;
	}
}