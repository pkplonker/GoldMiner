using System;
using System.Collections.Generic;
using Player;
using StuartHeathTools;
using UnityEngine;

namespace UI
{
	public class InventoryUI : CanvasGroupBase
	{
		[SerializeField] private GameObject _inventorySlotPrefab;
		private List<InventorySlotUI> _inventorySlots = new();
		private Inventory _inventory;
		[SerializeField] private PlayerReference _playerReference;
		[SerializeField] private Transform _container;
		private bool isActive;
		private void Awake()
		{
			Hide();
		}

		private void OnEnable()
		{
			PlayerReference.OnPlayerChanged += ChangeInventory;
			PlayerInputManager.OnInvent += ToggleVisability;
		}

		private void ToggleVisability()
		{
			if(isActive) Hide();
			else Show();
		}

		private void OnDisable()
		{
			PlayerReference.OnPlayerChanged -= ChangeInventory;
			PlayerInputManager.OnInvent -= ToggleVisability;

		}

		private void SetupInvent()
		{
			foreach (var slot in _inventorySlots)
			{
				Destroy(slot.gameObject);
			}

			_inventorySlots.Clear();
			for (var i = 0; i < _inventory.GetCapacity(); i++)
			{
				_inventorySlots.Add(Instantiate(_inventorySlotPrefab, _container).GetComponent<InventorySlotUI>());
				_inventorySlots[i].SetItem(_inventory.GetItem(i));
			}

			UpdateInventory();
		}

		private void ChangeInventory()
		{
			if (_playerReference.GetPlayer() == null) return;
			if (_inventory != null)
			{
				_inventory.OnInventoryChanged -= UpdateInventory;
				_inventory.OnInventorySetup -= SetupInvent;

			}

			_inventory = _playerReference.GetPlayer().GetComponent<Inventory>();
			_inventory.OnInventoryChanged += UpdateInventory;
			_inventory.OnInventorySetup += SetupInvent;

			SetupInvent();
		}

		private void UpdateInventory()
		{
			for (var i = 0; i < _inventorySlots.Count; i++)
			{
				_inventorySlots[i].SetItem(_inventory.GetItem(i));
			}
		}

		public void Show()
		{
			isActive = true;
			ShowUI();
		}

		public void Hide()
		{
			isActive = false;
			HideUI();
		}
	}
}