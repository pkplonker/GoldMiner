using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using Save;
using StuartHeathTools;
using Targets;
using UnityEngine;

public class GoldSpawnManager : TargetManager
{
	private PlayerCurrency playerCurrency;
	public event Action<Transform> GoldDeregistered;

	protected override void Awake() => ServiceLocator.Instance.RegisterService(this);

	private void AddGold(Target gold) => playerCurrency.AddGold(gold as Gold);

	public void Collect(Gold gold)
	{
		DeregisterTarget(gold);
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
		GoldDeregistered?.Invoke(gold.transform);
	}
}