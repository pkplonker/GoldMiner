using UnityEngine;

namespace TerrainGeneration
{
    public class TerrainChunk : MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshCollider _meshCollider;
        [SerializeField] private MeshRenderer _meshRenderer;
        public void Generate(MapData mapData, TerrainChunkData tcd)
        {
            var vertsPerRow = (mapData.MapChunkSize * mapData._lod) + 1;
            GenerateMesh(tcd, _meshFilter);
            GenerateCollider(_meshCollider, _meshFilter);
            GenerateMeshRenderer(tcd, _meshRenderer, vertsPerRow,mapData._material);
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
