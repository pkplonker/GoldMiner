//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using UnityEngine;

namespace Terrain
{
	/// <summary>
	///TileGeneration full description
	/// </summary>
	public class TileGeneration : MonoBehaviour
	{
		[SerializeField] private MeshRenderer tileRenderer;
		[SerializeField] private MeshFilter meshFilter;
		[SerializeField] private MeshCollider meshCollider;
		[SerializeField] private TerrainType[] terrainTypes;
		

		

		public void GenerateTile(int octaves,float persistance,float lacunarity,int seed, AnimationCurve heightCurve, float heightMultiplier, float mapScale) {
			// calculate tile depth and width based on the mesh vertices
			Vector3[] meshVertices = meshFilter.mesh.vertices;
			int tileDepth = (int)Mathf.Sqrt (meshVertices.Length);
			int tileWidth = tileDepth;
			// calculate the offsets based on the tile position
			float offsetX = -gameObject.transform.position.x;
			float offsetZ = -gameObject.transform.position.z;
			// generate a heightMap using noise
			float[,] heightMap = Noise.GenerateNoiseMap (tileDepth, tileWidth, mapScale, offsetX, offsetZ,  octaves,persistance,lacunarity,seed);
			// build a Texture2D from the height map
			Texture2D tileTexture = BuildTexture (heightMap);
			tileRenderer.material.mainTexture = tileTexture;
			// update the tile mesh vertices according to the height map
			UpdateMeshVertices (heightMap,heightCurve,heightMultiplier);
		}

		private void UpdateMeshVertices(float[,] heightMap, AnimationCurve heightCurve, float heightMultiplier) {
			var tileDepth = heightMap.GetLength (0);
			var tileWidth = heightMap.GetLength (1);
			Vector3[] meshVertices = meshFilter.mesh.vertices;
			// iterate through all the heightMap coordinates, updating the vertex index
			int vertexIndex = 0;
			for (int zIndex = 0; zIndex < tileDepth; zIndex++) {
				for (int xIndex = 0; xIndex < tileWidth; xIndex++) {
					float height = heightMap [zIndex, xIndex];
					Vector3 vertex = meshVertices [vertexIndex];
					// change the vertex Y coordinate, proportional to the height value. The height value is evaluated by the heightCurve function, in order to correct it.
					meshVertices[vertexIndex] = new Vector3(vertex.x, heightCurve.Evaluate(height) * heightMultiplier, vertex.z);
					vertexIndex++;
				}
			}
			// update the vertices in the mesh and update its properties
			meshFilter.mesh.vertices = meshVertices;
			meshFilter.mesh.RecalculateBounds ();
			meshFilter.mesh.RecalculateNormals ();
			// update the mesh collider
			meshCollider.sharedMesh = meshFilter.mesh;
		}

		
		private Texture2D BuildTexture(float[,] heightMap)
		{
			int tileDepth = heightMap.GetLength(0);
			int tileWidth = heightMap.GetLength(1);
			Color[] colorMap = new Color[tileDepth * tileWidth];
			for (int zIndex = 0; zIndex < tileDepth; zIndex++)
			{
				for (int xIndex = 0; xIndex < tileWidth; xIndex++)
				{
					// transform the 2D map index is an Array index
					int colorIndex = zIndex * tileWidth + xIndex;
					float height = heightMap[zIndex, xIndex];
					// choose a terrain type according to the height value
					TerrainType terrainType = ChooseTerrainType(height);
					// assign the color according to the terrain type
					colorMap[colorIndex] = terrainType.color;
				}
			}

			// create a new texture and set its pixel colors
			Texture2D tileTexture = new Texture2D(tileWidth, tileDepth);
			tileTexture.wrapMode = TextureWrapMode.Clamp;
			tileTexture.filterMode = FilterMode.Point;
			tileTexture.SetPixels(colorMap);
			tileTexture.Apply();
			return tileTexture;
		}

		TerrainType ChooseTerrainType(float height)
		{
			// for each terrain type, check if the height is lower than the one for the terrain type
			foreach (TerrainType terrainType in terrainTypes)
			{
				// return the first terrain type whose height is higher than the generated one
				if (height < terrainType.height)
				{
					return terrainType;
				}
			}
			return terrainTypes[^1];
		}
	}

	[Serializable]
	public class TerrainType
	{
		public float height;
		public Color color;
	}
}