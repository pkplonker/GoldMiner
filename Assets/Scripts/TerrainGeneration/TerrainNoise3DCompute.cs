using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class TerrainNoise3DCompute : IDisposable
{
	static readonly int OCTAVE_OFFSETS = Shader.PropertyToID("octaveOffsets");
	static readonly int RESULT = Shader.PropertyToID("Result");
	static readonly int PERSISTANCE = Shader.PropertyToID("persistance");
	static readonly int LACUNARITY = Shader.PropertyToID("lacunarity");
	static readonly int SCALE = Shader.PropertyToID("scale");
	static readonly int NOISE_EXTENTS = Shader.PropertyToID("noiseExtents");
	static readonly int VERTDISTANCE = Shader.PropertyToID("vertDistance");
	static readonly int OCTAVES = Shader.PropertyToID("octaves");
	static readonly int SIZE = Shader.PropertyToID("size");
	static readonly int WORLD_OFFSET = Shader.PropertyToID("worldOffset");
	static readonly int WORLD_CHUNK = Shader.PropertyToID("worldChunk");
	static readonly int MAX_CHUNK = Shader.PropertyToID("maxChunk");
	static readonly int GROUND_HEIGHT = Shader.PropertyToID("groundHeight");
	static readonly int ISOLEVEL = Shader.PropertyToID("isoLevel");
	static readonly int AMPLITUDE = Shader.PropertyToID("amplitude");

	const int THREAD_SIZE_X = 8;
	const int THREAD_SIZE_Y = 8;
	const int THREAD_SIZE_Z = 8;

	readonly ComputeShader shader;
	readonly int kernel;

	ComputeBuffer resultsBuffer;
	ComputeBuffer octaveOffsetsBuffer;

	float noiseExtents;

	public TerrainNoise3DCompute(ComputeShader shader)
	{
		this.shader = shader;
		kernel = shader.FindKernel("CSMain");
	}

	public void GenerateNoiseMap(
		Vector3Int dims,
		MarchingCubeMapData mapData,
		Vector3 offset,
		Action<float4[]> callback,
		AsyncQueue dispatchQ,
		AsyncQueue readbackQ,
		Vector3Int worldChunk,
		Vector3Int maxChunk)
	{
		Vector3[] octaveOffsets = CalculateOctaveOffsets(mapData.Octaves, offset, mapData.Seed);

		int voxelCount = dims.x * dims.y * dims.z;
		EnsureBuffers(mapData.Octaves, voxelCount);

		octaveOffsetsBuffer.SetData(octaveOffsets);

		noiseExtents = CalculateNoiseExtents(mapData) / (mapData.Octaves * 0.5f);

		SetShaderParameters(dims, mapData, offset);
		shader.SetFloats(WORLD_CHUNK, worldChunk.x, worldChunk.y, worldChunk.z);
		shader.SetFloats(MAX_CHUNK, maxChunk.x, maxChunk.y, maxChunk.z);

		int gx = (dims.x + THREAD_SIZE_X - 1) / THREAD_SIZE_X;
		int gy = (dims.y + THREAD_SIZE_Y - 1) / THREAD_SIZE_Y;
		int gz = (dims.z + THREAD_SIZE_Z - 1) / THREAD_SIZE_Z;

		shader.Dispatch(kernel, gx, gy, gz);

		dispatchQ.Register(() =>
		{
			AsyncGPUReadback.Request(resultsBuffer, req =>
			{
				if (req.hasError)
				{
					Debug.LogError("GPU read‑back error on resultsBuffer.");
					return;
				}

				var data = new float4[voxelCount];
				req.GetData<float4>().CopyTo(data);
				dispatchQ.Release();

				resultsBuffer.Release();
				resultsBuffer = null;
				octaveOffsetsBuffer.Release();
				octaveOffsetsBuffer = null;

				readbackQ.Register(() =>
				{
					callback?.Invoke(data);
					readbackQ.Release();
				});
			});
		});
	}

	/* ───── maths helpers (unchanged formulae) ───── */
	static float CalculateNoiseExtents(MarchingCubeMapData d)
	{
		float e = 0f, amp = d.Amplitude;
		for (int i = 0; i < d.Octaves; ++i)
		{
			e += amp;
			amp *= d.Persistance;
		}

		return e * 2.2f;
	}

	static Vector3[] CalculateOctaveOffsets(int octaves, Vector3 baseOffs, int seed)
	{
		var arr = new Vector3[octaves];
		for (int i = 0; i < octaves; ++i)
		{
			uint hX = math.hash(new int3(seed, i, 0));
			uint hY = math.hash(new int3(seed, i, 1));
			uint hZ = math.hash(new int3(seed, i, 2));

			const float k = 1.0f / uint.MaxValue;

			arr[i] = new Vector3(
				(hX * k * 200_000f - 100_000f) + baseOffs.x,
				(hY * k * 200_000f - 100_000f) + baseOffs.y,
				(hZ * k * 200_000f - 100_000f) + baseOffs.z);
		}

		return arr;
	}

	void EnsureBuffers(int octaves, int voxelCount)
	{
		if (octaveOffsetsBuffer == null || octaveOffsetsBuffer.count != octaves)
		{
			octaveOffsetsBuffer?.Release();
			octaveOffsetsBuffer = new ComputeBuffer(octaves, sizeof(float) * 3);
		}

		if (resultsBuffer == null || resultsBuffer.count != voxelCount)
		{
			resultsBuffer?.Release();
			resultsBuffer = new ComputeBuffer(voxelCount, sizeof(float) * 4);
		}
	}

	void SetShaderParameters(Vector3Int dims, MarchingCubeMapData d, Vector3 offs)
	{
		shader.SetBuffer(kernel, RESULT, resultsBuffer);
		shader.SetBuffer(kernel, OCTAVE_OFFSETS, octaveOffsetsBuffer);

		shader.SetFloat(NOISE_EXTENTS, noiseExtents);
		shader.SetFloat(PERSISTANCE, d.Persistance);
		shader.SetFloat(LACUNARITY, d.Lacunarity);
		shader.SetFloat(SCALE, d.Scale);
		shader.SetFloat(VERTDISTANCE, d.VertDistance);

		shader.SetInt(OCTAVES, d.Octaves);
		shader.SetInts(SIZE, dims.x, dims.y, dims.z);
		shader.SetFloats(WORLD_OFFSET, offs.x, offs.y, offs.z);
		shader.SetFloat(GROUND_HEIGHT, d.GroundHeight);
		shader.SetFloat(ISOLEVEL, d.IsoLevel);
		shader.SetFloat(AMPLITUDE, d.Amplitude);
	}

	public void Dispose()
	{
		resultsBuffer?.Release();
		octaveOffsetsBuffer?.Release();
		resultsBuffer = octaveOffsetsBuffer = null;
	}
}