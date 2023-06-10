using System;
using Player;
using UnityEngine;

namespace UI
{
	public class SellGoldButton : MonoBehaviour
	{
		[SerializeField] private PlayerReference PlayerReference;

		private void Awake() => PlayerReference.OnPlayerChanged += PlayerChanged;
		private void OnDestroy() => PlayerReference.OnPlayerChanged += PlayerChanged;


		private void PlayerChanged()
		{
		}

		//ui button
		public void SellGold()
		{
			if (PlayerReference == null || PlayerReference.GetPlayer() == null)
			{
				Debug.Log("no player");
				return;
			}

			var playerCurrency = PlayerReference.GetPlayer().GetComponent<PlayerCurrency>();
			if (!ExchangeGold(playerCurrency))
			{
				//todo do something constructive here
				Debug.Log("not enough gold");
				return;
			}
		}

		private bool ExchangeGold(PlayerCurrency playerCurrency)
		{
			if (playerCurrency.GetGold() == 0) return false;
			if (!playerCurrency.AddCurrency(playerCurrency.GetGold() * GoldPrice.goldPrice)) return false;
			playerCurrency.RemoveGold(playerCurrency.GetGold());
			return true;

		}
	}
}