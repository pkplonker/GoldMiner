using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "MapData")]
public class MarchingCubeMapData : ScriptableObject
{
	public float IsoLevel = 0.5f;
	public float VertDistance = 1.0f;
	public float Lacunarity = 0.7f;
	public int Seed = 0;
	public int Octaves = 8;
	public float Persistance = 1.7f;
	public float Scale = 2;
	public float GroundHeight = 2f;
	public float Amplitude = 30f;
	public string GroundLayer = "Ground";
	public float BoundaryInstep = 50;
	public Vector3Int MapSize = new Vector3Int(90, 9, 90);
	public Vector3Int ChunkSize = new Vector3Int(9, 9, 9);

	[HideInInspector]
	public int MapSize2D { get; private set; }

	public MarchingCubeMapData()
	{
		ChunkSize = new Vector3Int(9, 9, 9);
		MapSize2D = MapSize.x * MapSize.z;
	}
}