using System;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ChunkManager : MonoBehaviour, IService
{
	public Chunk[,,] Chunks { get; private set; }

	[SerializeField]
	private GameObject ChunkPrefab;

	private readonly Dictionary<Chunk, List<NoiseMapChange>> modifications = new();

	private AsyncQueue computeShaderQueue;

	[SerializeField]
	private ComputeShader NoiseShader;

	[SerializeField]
	private int MaxConcurrentGPUActions = 10;

	[SerializeField]
	private int MaxConcurrentGPUReadbackActions = 5;

	private AsyncQueue gpuAsyncReadbackqueue;
	private int generatedChunks;

	public Vector3Int maxChunkCoord { get; private set; }
	public Action TerrainGenerated { get; set; }
	public Action<int> TerrainGenerationStarted { get; set; }
	public Action<int, int> OnChunkGeneratedAction { get; set; }
	private MarchingCubeMapData mapData;

	private void OnEnable()
	{
		computeShaderQueue = new AsyncQueue("computeShaderQueue", () => MaxConcurrentGPUActions);
		gpuAsyncReadbackqueue = new AsyncQueue("gpuAsyncReadbackqueue", () => MaxConcurrentGPUReadbackActions);
		ServiceLocator.Instance.RegisterService<ChunkManager>(this);
	}

	public void Start()
	{
		Chunk.OnChunkGenerated += OnChunkGenerated;
	}

	private void OnChunkGenerated(Chunk obj)
	{
		generatedChunks++;
		var req = maxChunkCoord.x * maxChunkCoord.y * maxChunkCoord.z;

		OnChunkGeneratedAction?.Invoke(generatedChunks, req);
		if (generatedChunks == req)
		{
			TerrainGenerated?.Invoke();
		}
	}

	public void ClearChunks()
	{
		if (Chunks == null) return;
		for (var x = 0; x < Chunks.GetLength(0); x++)
		{
			for (var y = 0; y < Chunks.GetLength(1); y++)
			{
				for (var z = 0; z < Chunks.GetLength(2); z++)
				{
					if (Chunks[x, y, z] != null)
						Destroy(Chunks[x, y, z].gameObject);
				}
			}
		}

		generatedChunks = 0;
	}

	public void GenerateChunks(MarchingCubeMapData mapData)
	{
		// ---------- cache ----------
		this.mapData = mapData;
		var chunkSize = mapData.ChunkSize;
		var mapSize = mapData.MapSize;

		maxChunkCoord = new Vector3Int(
			(mapSize.x + chunkSize.x - 1) / chunkSize.x,
			(mapSize.y + chunkSize.y - 1) / chunkSize.y,
			(mapSize.z + chunkSize.z - 1) / chunkSize.z);

		int totalChunks = maxChunkCoord.x * maxChunkCoord.y * maxChunkCoord.z;
		TerrainGenerationStarted?.Invoke(totalChunks);

		Chunks = new Chunk[maxChunkCoord.x, maxChunkCoord.y, maxChunkCoord.z];

		Transform parent = transform;

		for (int x = 0; x < maxChunkCoord.x; ++x)
		{
			float posX = chunkSize.x * x;

			for (int y = 0; y < maxChunkCoord.y; ++y)
			{
				float posY = chunkSize.y * y;

				for (int z = 0; z < maxChunkCoord.z; ++z)
				{
					Vector3 localPos = new(posX, posY, chunkSize.z * z);

					GameObject chunkGO = Instantiate(ChunkPrefab, parent, false);
					chunkGO.transform.localPosition = localPos;

					var noise = new TerrainNoise3DCompute(NoiseShader);

					var chunk = chunkGO.GetComponent<Chunk>();
					chunk.Init(
						new Vector3Int(x, y, z),
						this,
						noise,
						chunkSize,
						mapData,
						computeShaderQueue,
						gpuAsyncReadbackqueue
					);

					Chunks[x, y, z] = chunk;
				}
			}
		}
	}

	private void Update()
	{
		computeShaderQueue.Tick();
		gpuAsyncReadbackqueue.Tick();
	}

	private void DrawSolidDebugChunk(int x, int y, int z)
	{
		var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		go.transform.localScale = this.mapData.ChunkSize;
		go.transform.position = new Vector3(this.mapData.ChunkSize.x * x, this.mapData.ChunkSize.y * y,
			this.mapData.ChunkSize.z * z);
		go.GetComponent<MeshRenderer>().material.color = new Color(UnityEngine.Random.value,
			UnityEngine.Random.value, UnityEngine.Random.value);
	}

	public bool Modify(Chunk chunk, RaycastHit hitInfo, float radius)
	{
		int factor = Mathf.CeilToInt(1f / mapData.VertDistance);
		Vector3 hitPoint = chunk.transform.InverseTransformPoint(hitInfo.point) * factor;

		Vector3Int paddedSize = (mapData.ChunkSize * factor) + new Vector3Int(1, 1, 1);

		int minX = Mathf.FloorToInt(hitPoint.x - radius * factor);
		int maxX = Mathf.CeilToInt(hitPoint.x + radius * factor);
		int minY = Mathf.FloorToInt(hitPoint.y - radius * factor);
		int maxY = Mathf.CeilToInt(hitPoint.y + radius * factor);
		int minZ = Mathf.FloorToInt(hitPoint.z - radius * factor);
		int maxZ = Mathf.CeilToInt(hitPoint.z + radius * factor);

		modifications[chunk] = new List<NoiseMapChange>();
		for (int x = minX; x < maxX; x++)
		{
			for (int y = minY; y < maxY; y++)
			{
				for (int z = minZ; z < maxZ; z++)
				{
					// Overflow to neighbour
					if (x >= paddedSize.x || y >= paddedSize.y || z >= paddedSize.z)
					{
						continue;
					}

					// Overflow to neighbour
					if (x < 0 || y < 0 || z < 0)
					{
						continue;
					}

					ProcessNeighbors(x, y, z, paddedSize, chunk, modifications);

					CreateModification(chunk, modifications, x, y, z, paddedSize);
				}
			}
		}

		foreach (var mod in modifications)
		{
			mod.Key.Modify(mod.Value);
		}

		modifications.Clear();
		return true;
	}

	private void ProcessNeighbors(int x, int y, int z, Vector3Int paddedSize, Chunk chunk,
		Dictionary<Chunk, List<NoiseMapChange>> modifications)
	{
		if (!IsOnEdgeOrCorner(x, y, z, paddedSize))
			return;

		int minX = x == 0 ? -1 : 0, maxX = x == paddedSize.x - 1 ? 1 : 0;
		int minY = y == 0 ? -1 : 0, maxY = y == paddedSize.y - 1 ? 1 : 0;
		int minZ = z == 0 ? -1 : 0, maxZ = z == paddedSize.z - 1 ? 1 : 0;

		for (int dx = minX; dx <= maxX; dx++)
		{
			for (int dy = minY; dy <= maxY; dy++)
			{
				for (int dz = minZ; dz <= maxZ; dz++)
				{
					if (dx == 0 && dy == 0 && dz == 0) continue;
					ProcessNeighborChunk(x, y, z, dx, dy, dz, paddedSize, chunk, modifications);
				}
			}
		}
	}

	private bool IsOnEdgeOrCorner(int x, int y, int z, Vector3Int paddedSize)
	{
		return x == 0 || x == paddedSize.x - 1 ||
		       y == 0 || y == paddedSize.y - 1 ||
		       z == 0 || z == paddedSize.z - 1;
	}

	private void ProcessNeighborChunk(int x, int y, int z, int dx, int dy, int dz, Vector3Int paddedSize, Chunk chunk,
		Dictionary<Chunk, List<NoiseMapChange>> modifications)
	{
		var chunkOffset = new Vector3Int(dx, dy, dz);
		var chunkIndex = chunk.ChunkCoord + chunkOffset;

		if (IsValidChunkIndex(chunkIndex, Chunks))
		{
			var neighbourChunk = Chunks[chunkIndex.x, chunkIndex.y, chunkIndex.z];
			var newX = (dx != 0) ? (dx == -1 ? paddedSize.x - 1 : 0) : x;
			var newY = (dy != 0) ? (dy == -1 ? paddedSize.y - 1 : 0) : y;
			var newZ = (dz != 0) ? (dz == -1 ? paddedSize.z - 1 : 0) : z;

			CreateModification(neighbourChunk, modifications, newX, newY, newZ, paddedSize);
		}
	}

	private bool IsValidChunkIndex(Vector3Int chunkIndex, Chunk[,,] chunks) =>
		chunkIndex.x >= 0 && chunkIndex.y >= 0 && chunkIndex.z >= 0 &&
		chunkIndex.x < chunks.GetLength(0) && chunkIndex.y < chunks.GetLength(1) &&
		chunkIndex.z < chunks.GetLength(2);

	private void CreateModification(Chunk chunk, Dictionary<Chunk, List<NoiseMapChange>> modifications, int x,
		int y, int z, Vector3Int paddedSize)
	{
		if (!modifications.ContainsKey(chunk))
		{
			modifications.Add(chunk, new List<NoiseMapChange>(20));
		}

		if (GetIndex(x, y, z, paddedSize) > paddedSize.x * paddedSize.y * paddedSize.z)
		{
			Debug.LogError("err");
		}

		modifications[chunk].Add(new NoiseMapChange
		{
			Index = GetIndex(x, y, z, paddedSize),
			Value = GetDigValue(),
		});
	}

	private static int GetIndex(int x, int y, int z, Vector3Int sz) =>
		x + y * sz.x + z * sz.x * sz.y;

	private float GetDigValue() => 1;

	public void Initialize() { }

	public Chunk GetChunkFromPosition(Vector3 result)
	{
		var x = Mathf.FloorToInt(result.x / mapData.ChunkSize.x);
		var y = Mathf.FloorToInt(result.y / mapData.ChunkSize.y);
		var z = Mathf.FloorToInt(result.z / mapData.ChunkSize.z);
		return Chunks[x, y, z];
	}
}