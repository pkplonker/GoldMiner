using System;
using UnityEngine;

namespace Terrain
{
	public class MapDisplay : MonoBehaviour
	{
		[SerializeField] private Renderer renderer;
		[SerializeField] private MeshFilter meshFilter;
		[SerializeField] private MeshRenderer meshRenderer;


		public void DrawTexture(Texture2D texture2D)
		{
			renderer.sharedMaterial.mainTexture = texture2D;
			renderer.transform.localScale = new Vector3(texture2D.width, 1, texture2D.height);
		}

		public void DrawMesh(MeshData meshData, Texture2D texture2D)
		{
			meshFilter.sharedMesh = meshData.CreateMesh();
			meshRenderer.sharedMaterial.mainTexture = texture2D;
		}
	}
}