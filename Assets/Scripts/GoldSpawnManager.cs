using System;
using System.Collections;
using System.Collections.Generic;
using StuartHeathTools;
using UnityEngine;

public class GoldSpawnManager : GenericUnitySingleton<GoldSpawnManager>
{
	private float _goldOnHand;
	private List<Gold> _goldPiecesSpawned = new List<Gold>();
	public static event Action<float,float, Vector3> OnGoldReceived; 
	private  List<Gold> _goldPiecesFound = new List<Gold>();

	public void RegisterGold(Gold gold)
	{
		if (!_goldPiecesSpawned.Contains(gold))
		{
			_goldPiecesSpawned.Add(gold);
		}
	}
	public void DeregisterGold(Gold gold)
	{
		if (_goldPiecesSpawned.Contains(gold))
		{
			_goldPiecesSpawned.Remove(gold);
		}
	}

	public void GoldCollected(Gold gold)
	{
		DeregisterGold(gold);
		_goldPiecesFound.Add(gold);
		_goldOnHand += gold.Weight;
		OnGoldReceived?.Invoke(gold.Weight,_goldOnHand, gold.transform.position);
	}


}