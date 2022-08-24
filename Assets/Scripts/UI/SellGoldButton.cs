using System;
using Player;
using UnityEngine;

namespace UI
{
	public class SellGoldButton : MonoBehaviour
	{
		[SerializeField] private PlayerReference _playerReference;

		private void Awake() => PlayerReference.OnPlayerChanged += PlayerChanged;
		private void OnDestroy() => PlayerReference.OnPlayerChanged += PlayerChanged;


		private void PlayerChanged()
		{
		}

		//ui button
		public void SellGold()
		{
			if (_playerReference == null || _playerReference.GetPlayer() == null)
			{
				Debug.Log("no player");
				return;
			}

			var playerCurrency = _playerReference.GetPlayer().GetComponent<PlayerCurrency>();
			ExchangeGold(playerCurrency);
		}

		private void ExchangeGold(PlayerCurrency playerCurrency)
		{
			if (playerCurrency.AddCurrency(playerCurrency.GetGold() * GoldPrice.goldPrice))
				playerCurrency.RemoveGold(playerCurrency.GetGold());
		}
	}
}