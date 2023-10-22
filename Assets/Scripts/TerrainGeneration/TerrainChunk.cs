using System;
using System.Collections.Generic;
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
			GenerateMeshRenderer(meshRenderer, vertsPerRow, mapData.material);
		}

		private void GenerateMesh(TerrainChunkData tcd, MeshFilter mf)
		{
			//Debug.Log($"{tcd.X}:{tcd.Y}");
			var mesh = new Mesh
			{
				name = $"X{tcd.X}:Y{tcd.Y}"
			};
			mesh.SetVertices(tcd.Verts);
			mesh.SetTriangles(tcd.Triangles, 0);
			mesh.RecalculateTangents();
			mesh.SetUVs(0, tcd.Uvs);
			//mesh.SetNormals(tcd.Normals);
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			//
			// Vector4[] tangents = new Vector4[tcd.Verts.Count];
			// for (int i = 0; i < tangents.Length; i++)
			// {
			// 	var vert = tcd.Verts[i];
			// 	tangents[i] = new Vector4(0f, 0f, 0f, 0f);
			// }
			//
			// mesh.SetTangents(tangents);
			mf.mesh = mesh;

			// for (int i = 0; i < mesh.vertices.Length; i++)
			// {
			// 	Debug.Log(mesh.vertices[i].y-mesh.tangents[i].y);
			// }
		}

		private static void GenerateCollider(MeshCollider mc, MeshFilter mf)
		{
			if (mc.sharedMesh) mc.sharedMesh.Clear();
			mc.sharedMesh = mf.mesh;
		}

		private void GenerateMeshRenderer(MeshRenderer mr, int vertsPerRow, Material material)
		{
			mr.material = new Material(material);
			mr.shadowCastingMode = ShadowCastingMode.Off;

			Texture2D texture = new Texture2D(vertsPerRow, vertsPerRow);
			Color[] pixels = new Color[vertsPerRow * vertsPerRow];

			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i] = new Color(0, 0, 0, 0);
			}

			texture.SetPixels(pixels);
			texture.Apply();

			material.mainTexture = texture;
		}

		public void ProcessNormalAlignment()
		{
			var terrainChunk = this;
			var x = terrainChunk.X;
			var y = terrainChunk.Y;

			if (y != MapGeneratorTerrain.terrainChunks.GetLength(1) - 1)
			{
				AlignEdgeNormals(Vector2.up, MapGeneratorTerrain.terrainChunks[x, y + 1]);
			}

			if (x != MapGeneratorTerrain.terrainChunks.GetLength(0) - 1)
			{
				AlignEdgeNormals(Vector2.right, MapGeneratorTerrain.terrainChunks[x + 1, y]);
			}

			if (y != 0)
			{
				AlignEdgeNormals(Vector2.down, MapGeneratorTerrain.terrainChunks[x, y - 1]);
			}

			if (x != 0)
			{
				AlignEdgeNormals(Vector2.left, MapGeneratorTerrain.terrainChunks[x - 1, y]);
			}
		}

		private void AlignEdgeNormals(Vector2 direction, TerrainChunk neighbour)
		{
			var mesh = GetMesh();
			var neighbourMesh = neighbour.GetMesh();

			Vector3[] normals = mesh.normals;
			Vector3[] neighbourNormals = neighbourMesh.normals;

			int resolution = MapData.lod * MapData.MapChunkSize + 1;

			for (int i = 0; i < resolution; i++)
			{
				int currentIndex, neighbourIndex;

				if (direction == Vector2.up)
				{
					currentIndex = (resolution - 1) * resolution + i;
					neighbourIndex = i;
				}
				else if (direction == Vector2.right)
				{
					currentIndex = i * resolution + (resolution - 1);
					neighbourIndex = i * resolution;
				}
				else if (direction == Vector2.down)
				{
					currentIndex = i;
					neighbourIndex = (resolution - 1) * resolution + i;
				}
				else if (direction == Vector2.left)
				{
					currentIndex = i * resolution;
					neighbourIndex = i * resolution + (resolution - 1);
				}
				else
				{
					return;
				}

				// Calculate the average normal
				Vector3 averageNormal = (normals[currentIndex] + neighbourNormals[neighbourIndex]).normalized;

				// Set the normals
				normals[currentIndex] = averageNormal;
				neighbourNormals[neighbourIndex] = averageNormal;
			}

			// Apply the changes
			mesh.SetNormals(normals);
			neighbourMesh.SetNormals(neighbourNormals);
		}

		private Mesh GetMesh()
		{
			if (meshFilter == null)
			{
				meshFilter = GetComponent<MeshFilter>();
			}

			return meshFilter.mesh;
		}
	}
}