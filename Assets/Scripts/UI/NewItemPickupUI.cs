using System;
using Player;
using Targets;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
	public class NewItemPickupUI : TweenUIPanel
	{
		[SerializeField] private TextMeshProUGUI _itemNameText;
		[SerializeField] private TextMeshProUGUI _valueText;
		[SerializeField] private Image _itemImage;
		private PickupTarget _pickupTarget;
		private PlayerInteractionStateMachine _playerInteractionStateMachine;
		private PlayerMovement _playerMovement;
		[SerializeField] private Button _keepButton;

	
		public void Show(PickupTarget pickupTarget, PlayerInteractionStateMachine playerInteractionStateMachine)
		{
			_playerInteractionStateMachine = playerInteractionStateMachine;
			_pickupTarget = pickupTarget;
			var item = _pickupTarget.GetItem();
			if (item == null)
			{
				Debug.LogError("Item is null");
				return;
			}

			_playerMovement = playerInteractionStateMachine.GetComponent<PlayerMovement>();
			if (_playerMovement == null)
			{
				Debug.LogError("Cannot inhibit player movement");
				return;
			}
			
			_playerMovement.SetCanMove(false);
			UpdateUI(item);
			ShowUI();
		}



		private void UpdateUI(Item item)
		{
			_itemNameText.text = item._itemName;
			_itemImage.sprite = item.Sprite;
			_valueText.text = item.GetValue().ToString();
		}

		//ui
		public void Pickup()
		{
			if (_pickupTarget == null || _playerInteractionStateMachine == null) Debug.LogError("missing refs");
			_pickupTarget.DestroyItem();
			var inv = _playerInteractionStateMachine.GetComponent<Inventory>();
			if (inv == null || !inv.Add(_pickupTarget.GetItem())) FailedToAddToInventory();
			Close();
		}

		private void FailedToAddToInventory() => Debug.LogError("Failed to add to inventory");

		//ui
		public void ThrowAway()
		{
			if (_pickupTarget == null) Debug.LogError("missing refs");
			_pickupTarget.DestroyItem();
			Close();
		}

		private void Close()=>HideUI(CloseCallback);
		

		private void CloseCallback() => _playerMovement.SetCanMove(true);
		public GameObject GetFirstSelectedObject() => _keepButton.gameObject;

		public void Show(float s = 0)
		{
			throw new NotImplementedException();
		}

		public void Hide(float s = 0)
		{
			throw new NotImplementedException();
		}
	}
}