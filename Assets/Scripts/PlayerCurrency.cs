//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using Newtonsoft.Json.Linq;
using Save;
using Targets;
using UnityEngine;

/// <summary>
///PlayerCurrency full description
/// </summary>
public class PlayerCurrency : MonoBehaviour, ISaveLoad
{
	[SerializeField] private FloatData goldAmount;
	[SerializeField] private FloatData currency;
	public static event Action<float, float> OnGoldChanged;
	public static event Action<float, float> OnCurrencyChanged;
	public float GetGold() => goldAmount.Value;
	public float GetCurrency() => currency.Value;

	public bool AddGold(Gold gold)
	{
		if (gold == null) return false;
		if (!ValidateInput(gold.Weight)) return false;
		goldAmount.Value += gold.Weight;
		OnGoldChanged?.Invoke(gold.Weight, goldAmount.Value);
		return true;
	}

	public bool AddGold(float gold)
	{
		if (!ValidateInput(gold)) return false;
		goldAmount.Value += gold;
		OnGoldChanged?.Invoke(gold, goldAmount.Value);
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
		goldAmount.Value -= amount;
		OnGoldChanged?.Invoke(amount * -1, goldAmount.Value);
		return true;
	}

	public bool AddCurrency(float amount)
	{
		if (!ValidateInput(amount)) return false;
		currency.Value += amount;
		OnCurrencyChanged?.Invoke(amount, currency.Value);
		return true;
	}

	private void SetCurrency(float amount)
	{
		currency.Value += amount;
		OnCurrencyChanged?.Invoke(amount, currency.Value);
	}

	private void SetGold(float gold)
	{
		goldAmount.Value += gold;
		OnGoldChanged?.Invoke(gold, goldAmount.Value);
	}

	public bool RemoveCurrency(float amount)
	{
		if (!ValidateInput(amount)) return false;
		currency.Value -= amount;
		OnCurrencyChanged?.Invoke(amount * -1, currency.Value);
		return true;
	}

	public void LoadState(object data)
	{
		if (data is JObject jobject)
		{
			try
			{
				var saveData = jobject.ToObject<SaveData>();
				currency.Value = 0;
				goldAmount.Value = 0;
				SetCurrency(saveData.currency);
				SetGold(saveData.gold);
			}
			catch (Exception ex)
			{
				Debug.LogError("Failed to deserialize SaveData: " + ex);
			}
		}
		else
		{
			Debug.LogError("Invalid data type passed to LoadState");
		}
	}

	public object SaveState() =>
		new SaveData
		{
			currency = currency.Value,
			gold = goldAmount.Value
		};

	private struct SaveData
	{
		public float currency;
		public float gold;
	}
}