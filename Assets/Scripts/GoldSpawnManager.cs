using System;
using System.Collections;
using System.Collections.Generic;
using StuartHeathTools;
using UnityEngine;

public class GoldSpawnManager : GenericUnitySingleton<GoldSpawnManager>
{
	private float _goldOnHand;
	public List<Gold> _goldPiecesSpawned { get; private set; }= new List<Gold>();
	public static event Action<float,float, Vector3> OnGoldReceived; 
	public  List<Gold> _goldPiecesFound { get; private set; }= new List<Gold>();

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