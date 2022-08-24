using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace UI
{
	public class GoldCounterUI : CounterUI
	{
		private void OnEnable() => GoldSpawnManager.OnGoldReceived += Received;
		private void OnDisable() => GoldSpawnManager.OnGoldReceived -= Received;
	}
}