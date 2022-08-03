using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace TerrainGeneration
{
	public class MapGenerator : MonoBehaviour
	{
		[field: SerializeField] public MapGeneratorTerrain MapGeneratorTerrain { get; private set; }
		[field: SerializeField] public PropSpawner PropSpawner { get; private set; }
		public static event Action<float> OnMapGenerated;
		public static event Action<int,int> OnMapGenerationStarted;




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
			MapGeneratorTerrain.OnTerrainGenerated += OnTerrainGenerated;
			PropSpawner.OnPropsGenerated += OnPropsGenerated;
		}

		private void OnTerrainGenerated()
		{
			Debug.Log("Terrain generated in " + mapTimer.ElapsedMilliseconds + "ms. Frames taken = " +
			          (Time.frameCount - startFrame));
			startFrame = Time.frameCount;
			propsTimer = new Stopwatch();

			propsTimer.Start();
		}

		private void OnPropsGenerated()
		{
			spawnedProps = true;
			
			Debug.Log("Props Spawned in " + propsTimer.ElapsedMilliseconds +
			          "ms . Frames taken = " +
			          (Time.frameCount - startFrame));
			OnMapGenerated?.Invoke(MapGeneratorTerrain.MapData.GetSize());
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
			OnMapGenerationStarted?.Invoke(chunksRequired,PropSpawner.GetPropsRequired());
			MapGeneratorTerrain.Generate();
		}

		private void Update()
		{
			if (MapGeneratorTerrain.Generated && !spawnedProps)
			{
				PropSpawner.SpawnObjects(MapGeneratorTerrain.MapData.GetSize(),
					MapGeneratorTerrain.MapData);
				spawnedProps = true;
			}
		}
	}
}