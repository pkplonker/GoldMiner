using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using StuartHeathTools;
using UnityEngine;

public class GoldSpawnManager : GenericUnitySingleton<GoldSpawnManager>
{
	public List<Gold> _goldPiecesSpawned { get; private set; } = new List<Gold>();
	public List<Gold> _goldPiecesFound { get; private set; } = new List<Gold>();
	[SerializeField] private PlayerReference _playerReference;
	private PlayerCurrency _playerCurrency;

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
		if (_playerCurrency != null) AddGold(gold);
		else if (_playerReference == null) Debug.Log("player reference missing");
		else
		{
			_playerCurrency = _playerReference.GetPlayer().GetComponent<PlayerCurrency>();
			if (_playerCurrency != null) AddGold(gold);
		}
	}

	private void AddGold(Gold gold) => _playerCurrency.AddGold(gold);
}