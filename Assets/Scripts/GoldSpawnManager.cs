using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using StuartHeathTools;
using Targets;
using UnityEngine;

public class GoldSpawnManager : GenericUnitySingleton<GoldSpawnManager>
{
	public List<Gold> goldPiecesSpawned { get; private set; } = new();
	public List<Gold> goldPiecesFound { get; private set; } = new();
	[SerializeField] private PlayerReference playerReference;
	private PlayerCurrency playerCurrency;
	

	public void RegisterGold(Gold gold)
	{
		if (!goldPiecesSpawned.Contains(gold)) goldPiecesSpawned.Add(gold);
	}

	public void DeregisterGold(Gold gold)
	{
		if (goldPiecesSpawned.Contains(gold)) goldPiecesSpawned.Remove(gold);
	}

	public void GoldCollected(Gold gold)
	{
		DeregisterGold(gold);
		if (playerCurrency != null)
		{
			AddGold(gold);
		}
		else if (playerReference == null)
		{
			Debug.Log("player reference missing");
		}
		else
		{
			playerCurrency = playerReference.GetPlayer().GetComponent<PlayerCurrency>();
			if (playerCurrency != null) AddGold(gold);
		}
	}

	private void AddGold(Gold gold) => playerCurrency.AddGold(gold);
}