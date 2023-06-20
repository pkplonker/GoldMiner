//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using StuartHeathTools;
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
	private float digAmount;
	private Dictionary<int, float> digChanges = new();
	[SerializeField] private Color dugGroundColorOffet;
	private TerrainChunk terrainChunk;
	private int terrainChunkLength;
	private HashSet<Tuple<int, int, int>> changes = new (100);
	private Mesh oldMesh;
	private List<Vector3> vertList = new ();
	private List<int> tris = new ();
	private List<Vector2> oldUvs = new ();
	private Vector3[] normals;
	private void OnEnable()=>Setup();
	

	public bool Dig(RaycastHit hit, float digAmount = 0.1f, float maxDigDepth = -2f)
	{
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
			colorMap[t] -= dugGroundColorOffet;
			colorMap[t] = colorMap[t].Clamp(new Color(0, 0, 0, 0), new Color(1, 1, 1, 1));
		}

		texture.SetPixels(colorMap);
		texture.Apply();
	}

	private void CheckNeighbours(RaycastHit hit)
	{
		changes.Clear();
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
			AddChange(changes, CheckTop(index, vertsPerRow, totalVerts), x, y + 1);
			AddChange(changes, CheckBottom(index, vertsPerRow, totalVerts), x, y - 1);
			AddChange(changes, CheckLeft(index, vertsPerRow), x - 1, y);
			AddChange(changes, CheckRight(index, vertsPerRow), x + 1, y);
			if (index == 0) AddChange(changes, totalVerts - 1, x - 1, y - 1);
			if (index == totalVerts - 1) AddChange(changes, 0, x + 1, y + 1);
			if (index == vertsPerRow - 1) AddChange(changes, totalVerts - vertsPerRow, x + 1, y - 1);
			if (index == totalVerts - vertsPerRow) AddChange(changes, vertsPerRow - 1, x - 1, y + 1);
		}

		ProcessNeighbourChanges();
	}

	private void AddChange(HashSet<Tuple<int, int, int>> changes, int val, int x, int y)
	{
		if (val != -1)
			changes.Add(new Tuple<int, int, int>(x, y, val));
	}
	private void ProcessNeighbourChanges()
	{

		foreach (var change in changes)
		{
			if (change.Item1 < 0 || change.Item1 > terrainChunkLength) continue;
			if (change.Item2 < 0 || change.Item2 > terrainChunkLength) continue;

			var dt = MapGeneratorTerrain.terrainChunks[change.Item1, change.Item2].GetComponent<DiggableTerrain>();
			if (dt != null) dt.DigAtVertIndex(change.Item3, digAmount);
		}
	}

	private void DigAtVertIndex(int index, float digAmount)
	{
		this.digAmount = digAmount;
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

	private int CheckRight(int index, int vertsPerRow) =>
		((index + 1) % vertsPerRow == 0) ? index - vertsPerRow + 1 : -1;

	private int CheckBottom(int index, int vertsPerRow, int totalVerts) =>
		index < vertsPerRow ? totalVerts - vertsPerRow + index : -1;

	private int CheckTop(int index, int vertsPerRow, int totalVerts) =>
		index >= totalVerts - vertsPerRow ? index % vertsPerRow : -1;

	private Vector3[] UpdateVerts(float digAmount, Vector3[] hitVerts, Vector3[] verts)
	{
		// Using a HashSet for constant time lookup
		HashSet<Vector3> hitVertsSet = new HashSet<Vector3>(hitVerts);

		for (var i = 0; i < verts.Length; i++)
		{
			var v = verts[i];
			if (hitVertsSet.Contains(v))
			{
				verts[i] = new Vector3(v.x, v.y - digAmount, v.z);
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
		oldMesh = meshFilter.mesh;
		var newMesh = new Mesh
		{
			name = oldMesh.name
		};

		vertList.Clear();
		vertList.AddRange(verts);
		newMesh.SetVertices(vertList);

		oldMesh.GetTriangles(tris, 0);
		newMesh.SetTriangles(tris, 0);

		oldUvs.Clear();
		oldMesh.GetUVs(0, oldUvs);
		newMesh.SetUVs(0, oldUvs);

		tris.Clear();
		tris.AddRange(newMesh.triangles);

		newMesh.SetNormals(oldMesh.normals);
		meshFilter.mesh = newMesh;
		oldMesh = newMesh;
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
		terrainChunk = GetComponent<TerrainChunk>();
		terrainChunkLength = MapGeneratorTerrain.terrainChunks.GetLength(0) - 1;

	}
}