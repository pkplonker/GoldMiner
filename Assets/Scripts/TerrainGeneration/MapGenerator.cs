using System;
using System.Diagnostics;
using Save;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace TerrainGeneration
{
	public class MapGenerator : MonoBehaviour, IService
	{
		[field: SerializeField] public MapGeneratorTerrain MapGeneratorTerrain { get; private set; }
		[field: SerializeField] public PropSpawner PropSpawner { get; private set; }
		public event Action<float> MapGenerated;
		public event Action<float, float> MapGeneratedTime;

		public event Action<MapData> TerrainGenerated;
		public event Action<int, int> MapGenerationStarted;

		private void Awake()
		{
			ServiceLocator.Instance.RegisterService(this);
		}

		public void RegenerateWorld(bool newSeed = false)
		{
			if (newSeed) MapGeneratorTerrain.MapData.seed = Random.Range(0, 32000);
			GenerateWorld();
		}

		public void RegenerateWorld(int newSeed)
		{
			MapGeneratorTerrain.MapData.seed = newSeed;
			GenerateWorld();
		}

		private void GenerateWorld()
		{
			MapGeneratorTerrain.ClearData();
			SpawnTerrain();
			spawnedProps = false;
		}

#if UNITY_EDITOR
		public bool spawnedProps { get; set; } = true;
		private Stopwatch mapTimer = new Stopwatch();
		private Stopwatch propsTimer = new Stopwatch();

		private int startFrame;
#else
		public bool spawnedProps { get; private set; }

#endif
		private void OnEnable()
		{
			MapGeneratorTerrain.OnTerrainGenerated += OnTerrainGenerated;
			PropSpawner.OnPropsGenerated += OnPropsGenerated;
		}

		private void OnDisable()
		{
			MapGeneratorTerrain.OnTerrainGenerated -= OnTerrainGenerated;
			PropSpawner.OnPropsGenerated -= OnPropsGenerated;
		}

		private void OnTerrainGenerated()
		{
#if UNITY_EDITOR
			Debug.Log("Terrain generated in " + mapTimer.ElapsedMilliseconds + "ms. Frames taken = " +
			          (Time.frameCount - startFrame));
			startFrame = Time.frameCount;
			propsTimer = new Stopwatch();
			propsTimer.Start();
#endif
			TerrainGenerated?.Invoke(MapGeneratorTerrain.MapData);
			if (PropSpawner.GetPropsRequired() == 0) OnPropsGenerated();
			else MapGeneratedTime?.Invoke(mapTimer.ElapsedMilliseconds, propsTimer.ElapsedMilliseconds);
		}

		private void OnPropsGenerated()
		{
			spawnedProps = true;
#if UNITY_EDITOR
			Debug.Log("Props Spawned in " + propsTimer.ElapsedMilliseconds +
			          "ms . Frames taken = " +
			          (Time.frameCount - startFrame));
#endif
			ServiceLocator.Instance.GetService<SavingSystem>().LoadGame();
			MapGenerated?.Invoke(MapGeneratorTerrain.MapData.GetSize());
			MapGeneratedTime?.Invoke(mapTimer.ElapsedMilliseconds, propsTimer.ElapsedMilliseconds);
		}

		public void SpawnTerrain()
		{
#if UNITY_EDITOR
			mapTimer = new Stopwatch();
			mapTimer.Start();
			startFrame = Time.frameCount;
#endif
			spawnedProps = false;
			var chunksRequired = MapGeneratorTerrain.MapData.ChunksPerRow * MapGeneratorTerrain.MapData.ChunksPerRow;
			MapGenerationStarted?.Invoke(chunksRequired, PropSpawner.GetPropsRequired());
			MapGeneratorTerrain.Generate();
		}

		private void Update()
		{
			if (MapGeneratorTerrain.Generated && !spawnedProps)
			{
				PropSpawner.SpawnObjects(MapGeneratorTerrain);
				spawnedProps = true;
			}
		}

		public void Initialize() { }

		public int GetSeed() => MapGeneratorTerrain.MapData.seed;
	}
}