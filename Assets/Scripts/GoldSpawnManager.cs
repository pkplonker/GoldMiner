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

	protected override void Awake()
	{
		ServiceLocator.Instance.RegisterService(this);
	}

	public override void DeregisterTarget(Target target)
	{
		base.DeregisterTarget(target);
		if (playerCurrency != null)
		{
			AddGold(target);
		}
		else if (playerReference == null)
		{
			Debug.Log("player reference missing");
		}
		else
		{
			playerCurrency = playerReference.GetPlayer().GetComponent<PlayerCurrency>();
			if (playerCurrency != null) AddGold(target);
		}
	}

	private void AddGold(Target gold) => playerCurrency.AddGold(gold as Gold);
}