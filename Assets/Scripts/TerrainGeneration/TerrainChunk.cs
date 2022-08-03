using UnityEngine;

namespace TerrainGeneration
{
	public class TerrainChunk : MonoBehaviour
	{
		[SerializeField] private MeshFilter _meshFilter;
		[SerializeField] private MeshCollider _meshCollider;
		[SerializeField] private MeshRenderer _meshRenderer;
		[field: SerializeField] public MapData MapData { get; private set; }
		[field: SerializeField] public int X { get; private set; }
		[field: SerializeField] public int Y { get; private set; }

		public void Generate(MapData mapData, TerrainChunkData tcd)
		{
			X = tcd.X;
			Y = tcd.Y;
			MapData = mapData;
			var vertsPerRow = (mapData.MapChunkSize * mapData._lod) + 1;
			GenerateMesh(tcd, _meshFilter);
			GenerateCollider(_meshCollider, _meshFilter);
			GenerateMeshRenderer(tcd, _meshRenderer, vertsPerRow, mapData._material);
			var mesh = _meshFilter.mesh;
			
			var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			go.name = "Dig hit point";
			go.transform.position = transform.TransformPoint(mesh.vertices[0]);
			go.transform.localScale = Vector3.one * 0.05f;
			go.GetComponent<Renderer>().material.color = Color.red;
			
			
			var go2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			go2.name = "Dig hit point";
			go2.transform.position =transform.TransformPoint( mesh.vertices[mapData.MapChunkSize*mapData._lod]);
			go2.transform.localScale = Vector3.one * 0.05f;
			go2.GetComponent<Renderer>().material.color = Color.black;
			
			var totalVerts = ((mapData.MapChunkSize*mapData._lod+1) * (mapData.MapChunkSize*mapData._lod+1))-1;
			
			var go3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			go3.name = "Dig hit point";
			go3.transform.position = transform.TransformPoint(mesh.vertices[totalVerts]);
			go3.transform.localScale = Vector3.one * 0.05f;
			go3.GetComponent<Renderer>().material.color = Color.green;
			
			var go1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			go1.name = "Dig hit point";
			go1.transform.position =transform.TransformPoint( mesh.vertices[totalVerts-mapData.MapChunkSize*mapData._lod]);
			go1.transform.localScale = Vector3.one * 0.05f;
			go1.GetComponent<Renderer>().material.color = Color.blue;
			

		}

		private static void GenerateMesh(TerrainChunkData tcd, MeshFilter mf)
		{
			var mesh = new Mesh()
			{
				name = $"X{tcd.X}:Y{tcd.Y}"
			};
			mesh.SetVertices(tcd.Verts);
			mesh.SetTriangles(tcd.Triangles, 0);
			mesh.SetNormals(tcd.Normals);
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
			mr.material.mainTexture = MapGeneratorTerrain.TextureFromColourMap(tcd.ColourMap, vertsPerRow);
		}
	}
}