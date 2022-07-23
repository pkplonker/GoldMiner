using UnityEngine;

namespace Terrain
{
	public class TerrainChunk : MonoBehaviour
	{
		[SerializeField] private MeshFilter meshFilter;
		[SerializeField] private MeshRenderer meshRenderer;
		[SerializeField] private MeshCollider meshCollider;
		public void DrawMesh(MeshData meshData, Texture2D texture2D)
		{
			meshFilter.mesh = meshData.CreateMesh();
			meshRenderer.material.mainTexture = texture2D;
			meshCollider.sharedMesh = meshFilter.mesh;
		}
	}
}
