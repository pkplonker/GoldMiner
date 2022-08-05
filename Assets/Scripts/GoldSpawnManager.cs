using System;
using System.Collections;
using System.Collections.Generic;
using StuartHeathTools;
using TerrainGeneration;
using UnityEngine;
using Random = UnityEngine.Random;

public class GoldSpawnManager : GenericUnitySingleton<GoldSpawnManager>
{
	public float GoldOnHand { get; private set; }
	private List<Gold> goldPiecesSpawned = new List<Gold>();
	public static event Action<float,float, Vector3> OnGoldReceived; 
	private List<Gold> goldPiecesFound = new List<Gold>();

	public void RegisterGold(Gold gold)
	{
		if (!goldPiecesSpawned.Contains(gold))
		{
			goldPiecesSpawned.Add(gold);
		}
	}
	public void DeregisterGold(Gold gold)
	{
		if (goldPiecesSpawned.Contains(gold))
		{
			goldPiecesSpawned.Remove(gold);
		}
	}

	public void GoldCollected(Gold gold)
	{
		DeregisterGold(gold);
		goldPiecesFound.Add(gold);
		GoldOnHand += gold.Weight;
		OnGoldReceived?.Invoke(gold.Weight,GoldOnHand, gold.transform.position);
	}


}