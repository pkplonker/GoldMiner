//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System.Collections.Generic;
using StuartHeathTools;
using TerrainGeneration;
using Unity.Collections;
using Unity.Jobs;
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
	private HashSet<TerrainChange> changes = new(100);
	private Mesh oldMesh;
	private List<Vector3> vertList = new();
	private List<int> tris = new();
	private List<Vector2> oldUvs = new();
	private Vector3[] normals;
	private Vector3[] hitVerts = new Vector3[3];
	private int[] hitVertIndexes = new int[3];
	private Material material;

	private void OnEnable() => Setup();

	public bool Dig(RaycastHit hit, float digAmount = 0.1f, float maxDigDepth = -2f)
	{
		this.digAmount = digAmount;
		var mesh = meshFilter.mesh;
		GetHitVerts(hit, mesh);
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
		UpdateColor(hitVertIndexes);
		return true;
	}

	private void UpdateColor(int[] hitVertIndexes)
	{
		material = meshRenderer.material;
		var texture = material.mainTexture as Texture2D;
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

	private void AddChange(HashSet<TerrainChange> changes, int val, int x, int y)
	{
		if (val != -1)
			changes.Add(new TerrainChange(x, y, val));
	}

	private void ProcessNeighbourChanges()
	{
		foreach (var change in changes)
		{
			if (change.X < 0 || change.X > terrainChunkLength) continue;
			if (change.Y < 0 || change.Y > terrainChunkLength) continue;

			var dt = MapGeneratorTerrain.terrainChunks[change.X, change.Y].GetComponent<DiggableTerrain>();
			if (dt != null) dt.DigAtVertIndex(change.Index, digAmount);
		}
	}

	private void DigAtVertIndex(int index, float digAmount)
	{
		this.digAmount = digAmount;
		var verts = meshFilter.mesh.vertices;
		verts[index].y -= digAmount;
		UpdateColor(new[] {index});
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
		var hitVertsNative = new NativeArray<Vector3>(hitVerts, Allocator.TempJob);
		var vertsNative = new NativeArray<Vector3>(verts, Allocator.TempJob);

		var job = new UpdateVertsJob
		{
			hitVerts = hitVertsNative,
			digAmount = digAmount,
			verts = vertsNative
		};

		var jobHandle = job.Schedule(vertsNative.Length, 64);
		jobHandle.Complete();

		vertsNative.CopyTo(verts);

		hitVertsNative.Dispose();
		vertsNative.Dispose();

		return verts;
	}

	private void UpdateCollider(Mesh newMesh)
	{
		//if (meshCollider.sharedMesh) meshCollider.sharedMesh.Clear();
		meshCollider.sharedMesh = newMesh;
		meshCollider.cookingOptions = MeshColliderCookingOptions.UseFastMidphase;
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

	private void GetHitVerts(RaycastHit hit, Mesh mesh)
	{
		var index = hit.triangleIndex;
		for (var i = 0; i < hitVerts.Length; i++)
		{
			hitVertIndexes[i] = mesh.triangles[index * 3 + i];
			hitVerts[i] = mesh.vertices[hitVertIndexes[i]];
		}
	}

	private void Setup()
	{
		if (!meshRenderer) meshRenderer = GetComponent<MeshRenderer>();
		if (!meshCollider) meshCollider = GetComponent<MeshCollider>();
		if (!meshFilter) meshFilter = GetComponent<MeshFilter>();
		terrainChunk = GetComponent<TerrainChunk>();
		terrainChunkLength = MapGeneratorTerrain.terrainChunks.GetLength(0) - 1;
		material = meshRenderer.material;
	}

	public struct TerrainChange
	{
		public int X;
		public int Y;
		public int Index;

		public TerrainChange(int x, int y, int index)
		{
			X = x;
			Y = y;
			Index = index;
		}
	}
}