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
	private MeshCollider meshCollider;
	private MeshRenderer meshRenderer;

	private MeshFilter meshFilter;
	private bool setup;
	private float digAmount;
	private Dictionary<int, float> digChanges = new();
	[SerializeField] private Color dugGroundColor;

	public bool Dig(RaycastHit hit, float digAmount = 0.1f, float maxDigDepth = -2f)
	{
		if (!setup) Setup();
		this.digAmount = digAmount;
		var mesh = meshFilter.mesh;
		var hitVerts = GetHitVerts(hit, mesh, out var hitVertIndexes);
		for (var i = 0; i < hitVerts.Length; i++)
		{
			AddToChanges(hit.triangleIndex * 3 + i, this.digAmount);
		}

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
		var mat = meshRenderer.material;
		var texture = mat.mainTexture as Texture2D;
		if (texture == null) return;
		var colorMap = texture.GetPixels();

//option 1
/*
		if (hitVertIndexes[0] > hitVertIndexes[1] && hitVertIndexes[0] > hitVertIndexes[2])
		{
			colorMap[hitVertIndexes[2]] = _dugGroundColor;
		}
		else
		{
			colorMap[hitVertIndexes[0]] = _dugGroundColor;
		}
		*/
//option 2
		foreach (var t in hitVertIndexes)
		{
			colorMap[t] = dugGroundColor;
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
			hitVertsIndexes[i] = meshFilter.mesh.triangles[triangleIndex * 3 + i];
		}

		var vertsPerRow = (mapData.MapChunkSize * mapData.lod) + 1;
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
			if (change.Item1 < 0 || change.Item1 > MapGeneratorTerrain.terrainChunks.GetLength(0) - 1) continue;
			if (change.Item2 < 0 || change.Item2 > MapGeneratorTerrain.terrainChunks.GetLength(1) - 1) continue;

			var dt = MapGeneratorTerrain.terrainChunks[change.Item1, change.Item2].GetComponent<DiggableTerrain>();
			if (dt != null) dt.DigAtVertIndex(change.Item3, digAmount);
		}
	}

	private void DigAtVertIndex(int index, float digAmount)
	{
		this.digAmount = digAmount;
		if (!setup) Setup();
		var verts = meshFilter.mesh.vertices;
		verts[index].y -= digAmount;
		UpdateColor(new[] {index}, verts);
		AddToChanges(index, digAmount);
		var updatedMesh = RegenerateMesh(verts);
		UpdateCollider(updatedMesh);
	}

	private void AddToChanges(int index, float digAmount)
	{
		if (digChanges.ContainsKey(index)) digChanges[index] += digAmount;
		else digChanges.TryAdd(index, digAmount);
	}

	private int CheckLeft(int index, int vertsPerRow) => (index % vertsPerRow == 0) ? index + vertsPerRow - 1 : -1;

	private int CheckRight(int index, int vertsPerRow) =>((index + 1) % vertsPerRow == 0) ? index - vertsPerRow + 1 : -1;

	private int CheckBottom(int index, int vertsPerRow, int totalVerts) => index < vertsPerRow ?totalVerts - vertsPerRow + index: -1;

	private int CheckTop(int index, int vertsPerRow, int totalVerts) => index >= totalVerts - vertsPerRow ? index % vertsPerRow : -1;

	private Vector3[] UpdateVerts(float digAmount, Vector3[] hitVerts, Vector3[] verts)
	{
		for (var i = 0; i < verts.Length; i++)
		{
			var v = verts[i];
			if (hitVerts.Any(hv => v == hv))
			{
				verts[i] = new Vector3(v.x, v.y - digAmount, v.z);
				//debug vert pos
				//	var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				//go.transform.position = transform.TransformPoint(verts[i]);
				//go.transform.localScale = Vector3.one * 0.1f;
			}
		}

		return verts;
	}

	private void UpdateCollider(Mesh newMesh)
	{
		if (meshCollider.sharedMesh) meshCollider.sharedMesh.Clear();
		meshCollider.sharedMesh = newMesh;
		meshCollider.cookingOptions = MeshColliderCookingOptions.EnableMeshCleaning;
	}

	private Mesh RegenerateMesh(Vector3[] verts)
	{
		var oldMesh = meshFilter.mesh;
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

		//newMesh.RecalculateNormals();

		newMesh.RecalculateBounds();
		meshFilter.mesh = newMesh;
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
		if (!meshRenderer) meshRenderer = GetComponent<MeshRenderer>();
		if (!meshCollider) meshCollider = GetComponent<MeshCollider>();
		if (!meshFilter) meshFilter = GetComponent<MeshFilter>();
		setup = meshCollider && meshFilter;
	}
}