//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using UnityEngine;

/// <summary>
///PlayerCurrency full description
/// </summary>
public class PlayerCurrency : MonoBehaviour
{
	private float _goldAmount;
	private float _currency;
	public static event Action<float, float> OnGoldChanged;
	public static event Action<float, float> OnCurrencyChanged;
	public float GetGold() => _goldAmount;
	public float GetCurrency() => _currency;

	public bool AddGold(Gold gold)
	{
		if (gold == null) return false;
		if (!ValidateInput(gold.Weight)) return false;
		_goldAmount += gold.Weight;
		OnGoldChanged?.Invoke(gold.Weight, _goldAmount);
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
		_goldAmount -= amount;
		OnGoldChanged?.Invoke(amount * -1, _goldAmount);
		return true;
	}

	public bool AddCurrency(float amount)
	{
		if (!ValidateInput(amount)) return false;
		_currency += amount;
		OnCurrencyChanged?.Invoke(amount, _currency);
		return true;
	}

	public bool RemoveCurrency(float amount)
	{
		if (!ValidateInput(amount)) return false;
		_currency -= amount;
		OnCurrencyChanged?.Invoke(amount * -1, _currency);
		return true;
	}
}