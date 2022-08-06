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
	private MeshRenderer _meshRenderer;

	private MeshFilter _meshFilter;
	private bool _setup = false;
	private float _digAmount;
	private Dictionary<int, float> digChanges = new Dictionary<int, float>();
	[SerializeField] private Color _dugGroundColor;
	private static readonly int MainTex = Shader.PropertyToID("_MainTex");

	public bool Dig(RaycastHit hit, float digAmount = 0.1f, float maxDigDepth = -2f)
	{
		if (!_setup) Setup();
		_digAmount = digAmount;
		var mesh = _meshFilter.mesh;
		var hitVerts = GetHitVerts(hit, mesh, out var hitVertIndexes);
		for (var i = 0; i < hitVerts.Length; i++)
		{
			AddToChanges(hit.triangleIndex * 3 + i, _digAmount);
		}

		var pass = false;
		var verts = UpdateVerts(digAmount, hitVerts, mesh.vertices);
		if (verts != null)
		{
			UpdateCollider(RegenerateMesh(verts));
		}

		CheckNeighbours(hit);
		UpdateColor(hitVertIndexes, verts);
		return true;
	}

	private void UpdateColor(int[] hitVertIndexes, Vector3[] verts)
	{
		var mat = _meshRenderer.material;
		var texture = mat.mainTexture as Texture2D;
		if (texture == null) return;
		var colorMap = texture.GetPixels();
		
		
		
		if (hitVertIndexes[0] > hitVertIndexes[1] && hitVertIndexes[0] > hitVertIndexes[2])
		{
			colorMap[hitVertIndexes[2]] = _dugGroundColor;
		}
		else
		{
			colorMap[hitVertIndexes[0]] = _dugGroundColor;

		}

		texture.SetPixels(colorMap);
		texture.Apply();
	}

	private void CheckNeighbours(RaycastHit hit)
	{
		var changes = new List<Tuple<int, int, int>>();
		var terrainChunk = GetComponent<TerrainChunk>();
		var mapData = terrainChunk.MapData;
		var x = terrainChunk.X;
		var y = terrainChunk.Y;
		var hitVertsIndexes = new int[3];
		var triangleIndex = hit.triangleIndex;

		for (var i = 0; i < hitVertsIndexes.Length; i++)
		{
			hitVertsIndexes[i] = _meshFilter.mesh.triangles[triangleIndex * 3 + i];
		}

		var vertsPerRow = (mapData.MapChunkSize * mapData._lod) + 1;
		var totalVerts = vertsPerRow * vertsPerRow;


		foreach (var index in hitVertsIndexes)
		{
			var val = CheckTop(index, vertsPerRow, totalVerts);
			if (val != -1) changes.Add(new Tuple<int, int, int>(x, y + 1, val));
			val = CheckBottom(index, vertsPerRow, totalVerts);
			if (val != -1) changes.Add(new Tuple<int, int, int>(x, y - 1, val));
			val = CheckLeft(index, vertsPerRow);
			if (val != -1) changes.Add(new Tuple<int, int, int>(x - 1, y, val));
			val = CheckRight(index, vertsPerRow);
			if (val != -1) changes.Add(new Tuple<int, int, int>(x + 1, y, val));
			if (index == 0) changes.Add(new Tuple<int, int, int>(x - 1, y - 1, totalVerts - 1));
			if (index == totalVerts - 1) changes.Add(new Tuple<int, int, int>(x + 1, y + 1, 0));
			if (index == vertsPerRow - 1) changes.Add(new Tuple<int, int, int>(x + 1, y - 1, totalVerts - vertsPerRow));
			if (index == totalVerts - vertsPerRow) changes.Add(new Tuple<int, int, int>(x - 1, y + 1, vertsPerRow - 1));
		}

		ProcessNeighbourChanges(changes);
	}

	private void ProcessNeighbourChanges(IEnumerable<Tuple<int, int, int>> changesRequested)
	{
		var changes = changesRequested.Distinct().ToList();


		foreach (var change in changes)
		{
			if (change.Item1 < 0 || change.Item1 > MapGeneratorTerrain.terrainChunks.GetLength(0)) continue;
			if (change.Item2 < 0 || change.Item2 > MapGeneratorTerrain.terrainChunks.GetLength(1)) continue;

			MapGeneratorTerrain.terrainChunks[change.Item1, change.Item2].GetComponent<DiggableTerrain>()
				.DigAtVertIndex(change.Item3, _digAmount);
		}
	}

	private void DigAtVertIndex(int index, float digAmount)
	{
		_digAmount = digAmount;
		if (!_setup) Setup();
		var verts = _meshFilter.mesh.vertices;
		verts[index].y -= digAmount;
		AddToChanges(index, digAmount);
		var updatedMesh = RegenerateMesh(verts);
		UpdateCollider(updatedMesh);
	}

	private void AddToChanges(int index, float digAmount)
	{
		if (digChanges.ContainsKey(index)) digChanges[index] += digAmount;
		else digChanges.TryAdd(index, digAmount);
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

	private Vector3[] UpdateVerts(float digAmount, Vector3[] hitVerts, Vector3[] verts)
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
		var tris = newMesh.triangles.ToList();
		TerrainChunkDataGenerator.CalculateNormals(ref vertList, out var normals, ref tris);
		newMesh.SetNormals(normals);
		newMesh.RecalculateBounds();
		_meshFilter.mesh = newMesh;
		return newMesh;
	}

	private static Vector3[] GetHitVerts(RaycastHit hit, Mesh mesh, out int[] hitVertIndexes)
	{
		var index = hit.triangleIndex;
		var hitVerts = new Vector3[3];
		hitVertIndexes = new int[3];
		for (var i = 0; i < hitVerts.Length; i++)
		{
			hitVertIndexes[i] = mesh.triangles[index * 3 + i];
			hitVerts[i] = (mesh.vertices[hitVertIndexes[i]]);
		}

		return hitVerts;
	}


	private void Setup()
	{
		if (!_meshRenderer) _meshRenderer = GetComponent<MeshRenderer>();
		if (!_meshCollider) _meshCollider = GetComponent<MeshCollider>();
		if (!_meshFilter) _meshFilter = GetComponent<MeshFilter>();
		_setup = _meshCollider && _meshFilter;
	}
}