//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using Props;
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
	private TerrainChunk terrainChunk;
	private float vertexColorFactor;

	public bool Dig(RaycastHit hit, float digAmount = 0.1f, float maxDigDepth = -2f)
	{
		if (!setup) Setup();
		this.digAmount = digAmount;
		vertexColorFactor = digAmount / SubSurfaceProp.globalMaxDepth;
		var mesh = meshFilter.mesh;
		var hitVertsIndexes = GetHitVerts(hit, mesh);

		var verts = UpdateVerts(digAmount, hitVertsIndexes, mesh.vertices);
		if (verts != null)
		{
			UpdateCollider(RegenerateMesh(verts));
		}

		CheckNeighbours(hitVertsIndexes);

		GetCurrentChunk().ProcessNormalAlignment();
		return true;
	}

	private void CheckNeighbours(int[] hitVertsIndexes)
	{
		var changes = new List<TerrainChange>();
		var terrainChunk = GetCurrentChunk();
		var mapData = terrainChunk.MapData;
		var x = terrainChunk.X;
		var y = terrainChunk.Y;

		var vertsPerRow = (mapData.MapChunkSize * mapData.lod) + 1;
		var totalVerts = vertsPerRow * vertsPerRow;

		foreach (var index in hitVertsIndexes)
		{
			var val = CheckTop(index, vertsPerRow, totalVerts);
			if (val != -1) changes.Add(new TerrainChange(x, y + 1, val));

			val = CheckBottom(index, vertsPerRow, totalVerts);
			if (val != -1) changes.Add(new TerrainChange(x, y - 1, val));

			val = CheckLeft(index, vertsPerRow);
			if (val != -1) changes.Add(new TerrainChange(x - 1, y, val));

			val = CheckRight(index, vertsPerRow);
			if (val != -1) changes.Add(new TerrainChange(x + 1, y, val));

			if (index == 0) changes.Add(new TerrainChange(x - 1, y - 1, totalVerts - 1));
			if (index == totalVerts - 1) changes.Add(new TerrainChange(x + 1, y + 1, 0));
			if (index == vertsPerRow - 1) changes.Add(new TerrainChange(x + 1, y - 1, totalVerts - vertsPerRow));
			if (index == totalVerts - vertsPerRow) changes.Add(new TerrainChange(x - 1, y + 1, vertsPerRow - 1));
		}

		ProcessNeighbourChanges(changes);
	}

	private TerrainChunk GetCurrentChunk() => terrainChunk == null ? GetComponent<TerrainChunk>() : terrainChunk;

	private void ProcessNeighbourChanges(IEnumerable<TerrainChange> changesRequested)
	{
		var changes = changesRequested.Distinct().ToList();

		Dictionary<DiggableTerrain, List<int>> batchedChanges = new Dictionary<DiggableTerrain, List<int>>();

		foreach (var change in changes)
		{
			if (change.X < 0 || change.X > MapGeneratorTerrain.terrainChunks.GetLength(0) - 1) continue;
			if (change.Y < 0 || change.Y > MapGeneratorTerrain.terrainChunks.GetLength(1) - 1) continue;

			var dt = MapGeneratorTerrain.terrainChunks[change.X, change.Y].GetComponent<DiggableTerrain>();
			if (dt == null) continue;
			if (!batchedChanges.ContainsKey(dt)) batchedChanges[dt] = new List<int>();

			batchedChanges[dt].Add(change.Index);
		}

		foreach (var kvp in batchedChanges)
		{
			var dt = kvp.Key;
			var indices = kvp.Value.ToArray();
			dt.DigAtVertIndices(indices, digAmount);
		}
	}

	private void DigAtVertIndices(int[] indices, float digAmount)
	{
		this.digAmount = digAmount;
		if (!setup) Setup();
		var verts = meshFilter.mesh.vertices;

		foreach (var index in indices)
		{
			if (meshFilter.mesh.tangents[index].y < 1)
				verts[index].y -= digAmount;
		}

		var updatedMesh = RegenerateMesh(verts);
		UpdateCollider(updatedMesh);
	}

	private int CheckLeft(int index, int vertsPerRow) => (index % vertsPerRow == 0) ? index + vertsPerRow - 1 : -1;

	private int CheckRight(int index, int vertsPerRow) =>
		((index + 1) % vertsPerRow == 0) ? index - vertsPerRow + 1 : -1;

	private int CheckBottom(int index, int vertsPerRow, int totalVerts) =>
		index < vertsPerRow ? totalVerts - vertsPerRow + index : -1;

	private int CheckTop(int index, int vertsPerRow, int totalVerts) =>
		index >= totalVerts - vertsPerRow ? index % vertsPerRow : -1;

	private Mesh RegenerateMesh(Vector3[] newVerts)
	{
		vertexColorFactor = digAmount / SubSurfaceProp.globalMaxDepth;
		var oldMesh = meshFilter.mesh;
		var oldVerts = oldMesh.vertices;

		var newMesh = new Mesh
		{
			name = oldMesh.name
		};
		newMesh.SetVertices(newVerts);
		newMesh.triangles = oldMesh.GetTriangles(0);

		var oldUvs = new List<Vector2>();
		oldMesh.GetUVs(0, oldUvs);
		newMesh.SetUVs(0, oldUvs);
		newMesh.RecalculateNormals();
		var tangents = new List<Vector4>();
		oldMesh.GetTangents(tangents);

		for (int i = 0; i < oldVerts.Length; i++)
		{
			float yDifference = oldVerts[i].y - newVerts[i].y > 0 ? vertexColorFactor : 0;
			tangents[i] = new Vector4(tangents[i].x, tangents[i].y + yDifference, tangents[i].z, tangents[i].w);
		}

		newMesh.SetTangents(tangents);
		newMesh.RecalculateBounds();

		meshFilter.mesh = newMesh;
		return newMesh;
	}

	private Color? col = null;

	private void OnDrawGizmosSelected()
	{
		if (meshFilter == null) meshFilter = GetComponent<MeshFilter>();
		if (col == null)
		{
			col = new Color(UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f),
				UnityEngine.Random.Range(0, 1f));
		}
	
		Mesh mesh = meshFilter.mesh;
		Vector3[] vertices = mesh.vertices;
		Vector3[] normals = mesh.normals;
		Vector4[] tangents = mesh.tangents;
		Transform transform = meshFilter.transform;
	
		for (int i = 0; i < vertices.Length; i++)
		{
			Vector3 worldVertex = transform.TransformPoint(vertices[i]);
			Vector3 worldNormal = transform.TransformDirection(normals[i]);
			Vector3 worldTangent = transform.TransformDirection(tangents[i]);
	
			Debug.DrawRay(worldVertex, worldNormal * 0.1f, col.Value);
			// if (tangents[i] == new Vector4(1,0,0,1)) continue;
			// Debug.DrawRay(worldVertex, worldTangent * 3f, Color.red);
		}
	}

	private Vector3[] UpdateVerts(float digAmount, int[] hitVerts, Vector3[] verts)
	{
		foreach (var hit in hitVerts)
		{
			if (meshFilter.mesh.tangents[hit].y < 1)
				verts[hit] -= new Vector3(0, digAmount, 0);
			else
			{
				Debug.Log("max dig");
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

	private int[] GetHitVerts(RaycastHit hit, Mesh mesh)
	{
		var index = hit.triangleIndex;
		var hitVerts = new Vector3[6];
		var hitVertIndexes = new int[6];

		for (var i = 0; i < 3; i++)
		{
			hitVertIndexes[i] = mesh.triangles[index * 3 + i];
			hitVerts[i] = mesh.vertices[hitVertIndexes[i]];
		}

		int adjacentIndex = index % 2 == 0 ? index + 1 : index - 1;
		if (adjacentIndex != -1)
		{
			for (var i = 0; i < 3; i++)
			{
				hitVertIndexes[i + 3] = mesh.triangles[adjacentIndex * 3 + i];
				hitVerts[i + 3] = mesh.vertices[hitVertIndexes[i + 3]];
			}
		}

		return hitVertIndexes.Distinct().ToArray();
	}

	private void Setup()
	{
		if (!meshRenderer) meshRenderer = GetComponent<MeshRenderer>();
		if (!meshCollider) meshCollider = GetComponent<MeshCollider>();
		if (!meshFilter) meshFilter = GetComponent<MeshFilter>();
		setup = meshRenderer && meshCollider && meshFilter;
	}

	private struct TerrainChange
	{
		public int X { get; set; }
		public int Y { get; set; }
		public int Index { get; set; }

		public TerrainChange(int x, int y, int index)
		{
			X = x;
			Y = y;
			Index = index;
		}

		public override bool Equals(object obj)
		{
			if (obj is TerrainChange other)
			{
				return X == other.X && Y == other.Y && Index == other.Index;
			}

			return false;
		}

		public override int GetHashCode() => HashCode.Combine(X, Y, Index);
	}
}