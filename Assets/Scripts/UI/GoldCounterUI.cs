using System;
using System.Collections;
using Player;
using TMPro;
using UnityEngine;

namespace UI
{
	public class GoldCounterUI : CounterUI
	{
		protected override void UnSubscribe()=>PlayerCurrency.OnGoldChanged -= Received;
		
		protected override void Subscribe() => PlayerCurrency.OnGoldChanged += Received;
	}
}