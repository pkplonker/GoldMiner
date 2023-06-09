//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using Targets;
using UnityEngine;

/// <summary>
///PlayerCurrency full description
/// </summary>
public class PlayerCurrency : MonoBehaviour
{
	private float goldAmount;
	private float currency;
	public static event Action<float, float> OnGoldChanged;
	public static event Action<float, float> OnCurrencyChanged;
	public float GetGold() => goldAmount;
	public float GetCurrency() => currency;

	public bool AddGold(Gold gold)
	{
		if (gold == null) return false;
		if (!ValidateInput(gold.Weight)) return false;
		goldAmount += gold.Weight;
		OnGoldChanged?.Invoke(gold.Weight, goldAmount);
		return true;
	}

	private static bool ValidateInput(float amount)
	{
		if (!(amount <= 0)) return true;
		Debug.Log("Tried changing by invalid amount");
		return false;
	}

	public bool RemoveGold(float amount)
	{
		if (!ValidateInput(amount)) return false;
		goldAmount -= amount;
		OnGoldChanged?.Invoke(amount * -1, goldAmount);
		return true;
	}

	public bool AddCurrency(float amount)
	{
		if (!ValidateInput(amount)) return false;
		currency += amount;
		OnCurrencyChanged?.Invoke(amount, currency);
		return true;
	}

	public bool RemoveCurrency(float amount)
	{
		if (!ValidateInput(amount)) return false;
		currency -= amount;
		OnCurrencyChanged?.Invoke(amount * -1, currency);
		return true;
	}
}