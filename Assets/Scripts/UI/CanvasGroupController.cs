using System;
using System.Collections.Generic;
using Player;
using StuartHeathTools;
using Targets;
using UnityEngine;

namespace UI
{
	public class CanvasGroupController : GenericUnitySingleton<CanvasGroupController>
	{
		[SerializeField] private NewItemPickupUI _newItemPickupUIPrefab;
		private NewItemPickupUI _newItemPickupUI;
		[SerializeField] private HUDUI _hud;
		[SerializeField] private InventoryUI _inventory;
		 private TruckUI _truckUI;
		[SerializeField] private TruckUI _truckUIPrefab;
		private bool inventActive = false;
		private void OnEnable() => PlayerInputManager.OnInvent += ToggleInventory;

		private void ToggleInventory()
		{
			if (inventActive) HideInventory();
			else ShowInventory();
		}

		private void OnDisable() => PlayerInputManager.OnInvent -= ToggleInventory;

		private void Start()
		{
			ShowHUD();
			HideInventory();
		}


		public void ShowNewItemPickupUI(PickupTarget pickupTarget, PlayerInteractionStateMachine player)
		{
			if (_newItemPickupUI == null)
			{
				_newItemPickupUI = Instantiate(_newItemPickupUIPrefab, transform);
			}

			_newItemPickupUI.Show(pickupTarget, player);
		}

		public void ShowHUD()
		{
			_hud.Show();
		}

		public void ShowInventory()
		{
			_inventory.Show();
			inventActive = true;
		}

		private void HideInventory()
		{
			_inventory.Hide();
			inventActive = false;
		}

		public void ShowTruckUI(PlayerInteractionStateMachine player)
		{
			if (_truckUI == null)
			{
				_truckUI = Instantiate(_truckUIPrefab, transform);
			}

			_truckUI.Show(player);
		}

		public void HideTruckUI()
		{
			_truckUI.Hide();
		}
	}
}