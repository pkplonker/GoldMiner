using System;
using Player;
using UnityEngine;

namespace UI
{
	public class SellGoldButton : MonoBehaviour
	{
		[SerializeField] private PlayerReference playerReference;

		private void Awake() => PlayerReference.OnPlayerChanged += PlayerChanged;
		private void OnDestroy() => PlayerReference.OnPlayerChanged += PlayerChanged;


		private void PlayerChanged()
		{
		}

		//ui button
		public void SellGold()
		{
			if (playerReference == null || playerReference.GetPlayer() == null)
			{
				Debug.Log("no player");
				return;
			}

			var playerCurrency = playerReference.GetPlayer().GetComponent<PlayerCurrency>();
			ExchangeGold(playerCurrency);
		}

		private void ExchangeGold(PlayerCurrency playerCurrency)
		{
			if (playerCurrency.AddCurrency(playerCurrency.GetGold() * GoldPrice.goldPrice))
				playerCurrency.RemoveGold(playerCurrency.GetGold());
		}
	}
}