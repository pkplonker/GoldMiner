using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace TerrainGeneration
{
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshCollider))]
	public class TerrainChunk : MonoBehaviour
	{
		[SerializeField] private MeshFilter meshFilter;
		[SerializeField] private MeshCollider meshCollider;
		[SerializeField] private MeshRenderer meshRenderer;
		public MapData MapData { get; private set; }
		[field: SerializeField] public int X { get; private set; }
		[field: SerializeField] public int Y { get; private set; }

		private void OnValidate()
		{
			meshFilter = GetComponent<MeshFilter>();
			meshCollider = GetComponent<MeshCollider>();
			meshRenderer = GetComponent<MeshRenderer>();
		}

		public void Generate(MapData mapData, TerrainChunkData tcd)
		{
			X = tcd.X;
			Y = tcd.Y;
			MapData = mapData;
			var vertsPerRow = (mapData.MapChunkSize * mapData.lod) + 1;
			GenerateMesh(tcd, meshFilter);
			GenerateCollider(meshCollider, meshFilter);
			GenerateMeshRenderer(tcd, meshRenderer, vertsPerRow, mapData.material);
			meshFilter.mesh.RecalculateNormals();
		}

		private static void GenerateMesh(TerrainChunkData tcd, MeshFilter mf)
		{
			var mesh = new Mesh()
			{
				name = $"X{tcd.X}:Y{tcd.Y}"
			};
			mesh.SetVertices(tcd.Verts);
			mesh.SetTriangles(tcd.Triangles, 0);
			//mesh.SetNormals(tcd.Normals);
			mesh.RecalculateTangents();
			//mesh.RecalculateNormals();
			
			mesh.SetUVs(0, tcd.Uvs);
			mesh.RecalculateBounds();
			mf.mesh = mesh;
		}

		private static void GenerateCollider(MeshCollider mc, MeshFilter mf)
		{
			if (mc.sharedMesh) mc.sharedMesh.Clear();
			mc.sharedMesh = mf.mesh;
			//mc.cookingOptions = MeshColliderCookingOptions.EnableMeshCleaning;
		}

		private void GenerateMeshRenderer(TerrainChunkData tcd, MeshRenderer mr, int vertsPerRow, Material material)
		{
			mr.material = material;
			//mr.material.mainTexture = MapGeneratorTerrain.TextureFromColourMap(tcd.ColourMap, vertsPerRow);
			mr.shadowCastingMode = ShadowCastingMode.Off;
		}
	}
}