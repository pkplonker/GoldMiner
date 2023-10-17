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
	[SerializeField] private Color dugGroundColorOffet = Color.black;

	public bool Dig(RaycastHit hit, float digAmount = 0.1f, float maxDigDepth = -2f)
	{
		if (!setup) Setup();
		this.digAmount = digAmount;
		var mesh = meshFilter.mesh;
		var hitVerts = GetHitVerts(hit, mesh, out var hitVertIndexes);

		var verts = UpdateVerts(digAmount, hitVerts, mesh.vertices);
		if (verts != null)
		{
			UpdateCollider(RegenerateMesh(verts));
		}

		CheckNeighbours(hit);
		UpdateColor(hitVertIndexes, verts);
		return true;
	}

	private void UpdateColor(int[] hitVertIndexes, Vector3[] verts, bool modifyNeighbours = true)
	{
		var mat = meshRenderer.material;
		var originalTexture = mat.mainTexture as Texture2D;
		if (originalTexture == null) return;

		int textureWidth = originalTexture.width;
		Color halfStrengthColor = dugGroundColorOffet * 0.5f;

		foreach (var t in hitVertIndexes)
		{
			int x = t % textureWidth;
			int y = t / textureWidth;

			int xMin = Mathf.Max(x - 1, 0);
			int xMax = Mathf.Min(x + 1, textureWidth - 1);
			int yMin = Mathf.Max(y - 1, 0);
			int yMax = Mathf.Min(y + 1, originalTexture.height - 1);

			Color[] regionPixels = originalTexture.GetPixels(xMin, yMin, xMax - xMin + 1, yMax - yMin + 1);

			regionPixels[(y - yMin) * (xMax - xMin + 1) + (x - xMin)] = dugGroundColorOffet;

			if (modifyNeighbours)
			{
				if (x > xMin) regionPixels[(y - yMin) * (xMax - xMin + 1) + (x - xMin - 1)] = halfStrengthColor; // Left
				if (x < xMax)
					regionPixels[(y - yMin) * (xMax - xMin + 1) + (x - xMin + 1)] = halfStrengthColor; // Right
				if (y > yMin)
					regionPixels[(y - yMin - 1) * (xMax - xMin + 1) + (x - xMin)] = halfStrengthColor; // Below
				if (y < yMax)
					regionPixels[(y - yMin + 1) * (xMax - xMin + 1) + (x - xMin)] = halfStrengthColor; // Above
			}

			originalTexture.SetPixels(xMin, yMin, xMax - xMin + 1, yMax - yMin + 1, regionPixels);
		}

		originalTexture.Apply();
	}

	private void CheckNeighbours(RaycastHit hit)
	{
		var changes = new List<TerrainChange>();
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
			verts[index].y -= digAmount;
		}

		UpdateColor(indices, verts);
		var updatedMesh = RegenerateMesh(verts);
		UpdateCollider(updatedMesh);
	}

	private Mesh RegenerateMesh(Vector3[] verts)
	{
		var oldMesh = meshFilter.mesh;
		var newMesh = new Mesh
		{
			name = oldMesh.name
		};
		newMesh.SetVertices(verts);
		newMesh.triangles = oldMesh.GetTriangles(0);
		var oldUvs = new List<Vector2>();
		oldMesh.GetUVs(0, oldUvs);
		newMesh.SetUVs(0, oldUvs);
		//newMesh.normals = oldMesh.normals;
		newMesh.RecalculateNormals();
		newMesh.RecalculateBounds();
		meshFilter.mesh = newMesh;
		return newMesh;
	}

	private Vector3[] UpdateVerts(float digAmount, Vector3[] hitVerts, Vector3[] verts)
	{
		for (var i = 0; i < verts.Length; i++)
		{
			var v = verts[i];
			if (hitVerts.Any(hv => v == hv)) verts[i] = new Vector3(v.x, v.y - digAmount, v.z);
		}

		return verts;
	}

	private void UpdateCollider(Mesh newMesh)
	{
		if (meshCollider.sharedMesh) meshCollider.sharedMesh.Clear();
		meshCollider.sharedMesh = newMesh;
		meshCollider.cookingOptions = MeshColliderCookingOptions.EnableMeshCleaning;
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

	private int CheckLeft(int index, int vertsPerRow) => (index % vertsPerRow == 0) ? index + vertsPerRow - 1 : -1;

	private int CheckRight(int index, int vertsPerRow) =>
		((index + 1) % vertsPerRow == 0) ? index - vertsPerRow + 1 : -1;

	private int CheckBottom(int index, int vertsPerRow, int totalVerts) =>
		index < vertsPerRow ? totalVerts - vertsPerRow + index : -1;

	private int CheckTop(int index, int vertsPerRow, int totalVerts) =>
		index >= totalVerts - vertsPerRow ? index % vertsPerRow : -1;

	private void Setup()
	{
		if (!meshRenderer) meshRenderer = GetComponent<MeshRenderer>();
		if (!meshCollider) meshCollider = GetComponent<MeshCollider>();
		if (!meshFilter) meshFilter = GetComponent<MeshFilter>();
		setup = meshCollider && meshFilter;
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