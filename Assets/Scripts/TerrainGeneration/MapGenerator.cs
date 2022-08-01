using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace TerrainGeneration
{
	public class MapGenerator : MonoBehaviour
	{
		[field: SerializeField] public MapGeneratorTerrain mapGeneratorTerrain { get; private set; }
		[field: SerializeField] public PropSpawner propSpawner { get; private set; }
		public static event Action<float> OnMapGenerated;

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
			OnMapGenerated?.Invoke(mapGeneratorTerrain.mapData.GetSize());
		} 


		public void SpawnTerrain()
		{
#if UNITY_EDITOR
			mapTimer = new Stopwatch();
			mapTimer.Start();
			startFrame = Time.frameCount;
#endif

			mapGeneratorTerrain.Generate();
		}

		private void Update()
		{
			if (mapGeneratorTerrain.generated && !spawnedProps)
			{
				propSpawner.SpawnObjects(mapGeneratorTerrain.mapData.GetSize(),
					mapGeneratorTerrain.mapData);
				spawnedProps = true;
			}
		}
	}
}