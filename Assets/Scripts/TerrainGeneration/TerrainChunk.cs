using UnityEngine;

namespace TerrainGeneration
{
    public class TerrainChunk : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshCollider meshCollider;
        [SerializeField] private MeshRenderer meshRenderer;
        public void Generate(MapData mapData, TerrainChunkData tcd)
        {
            var vertsPerRow = (mapData.mapChunkSize * mapData.LOD) + 1;
            GenerateMesh(tcd, meshFilter);
            GenerateCollider(meshCollider, meshFilter);
            GenerateMeshRenderer(tcd, meshRenderer, vertsPerRow,mapData.material);
        }
        private static void GenerateMesh(TerrainChunkData tcd, MeshFilter mf)
        {
            var mesh = new Mesh()
            {
                name = $"X{tcd.x}:Y{tcd.y}"
            };
            mesh.SetVertices(tcd.verts);
            mesh.SetTriangles(tcd.triangles, 0);
            mesh.SetNormals(tcd.normals);
            mesh.SetUVs(0, tcd.uvs);
            mesh.RecalculateBounds();
            mf.mesh = mesh;
        }

        private static void GenerateCollider(MeshCollider mc, MeshFilter mf)
        {
            if (mc.sharedMesh) mc.sharedMesh.Clear();
            mc.sharedMesh = mf.mesh;
            mc.cookingOptions = MeshColliderCookingOptions.EnableMeshCleaning;
        }

        private void GenerateMeshRenderer(TerrainChunkData tcd, MeshRenderer mr, int vertsPerRow, Material material)
        {
            mr.material = material;
            mr.material.mainTexture = MapGeneratorTerrain.TextureFromColourMap(tcd.colourMap, vertsPerRow);
        }
    }
}
