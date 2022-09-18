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

		private void Close()=>HideUI();

		public override void Show()
		{
			base.Show();
			EventSystem.current.SetSelectedGameObject(_keepButton.gameObject);
		}

		public override void Hide()
		{
			base.Hide();
			_playerMovement.SetCanMove(true);
		}


		public void Init(PickupTarget pickupTarget, PlayerInteractionStateMachine player)
		{
			_playerInteractionStateMachine = player;
			_pickupTarget = pickupTarget;
			var item = _pickupTarget.GetItem();
			if (item == null)
			{
				Debug.LogError("Item is null");
				return;
			}

			_playerMovement = _playerInteractionStateMachine.GetComponent<PlayerMovement>();
			if (_playerMovement == null)
			{
				Debug.LogError("Cannot inhibit player movement");
				return;
			}
			
			_playerMovement.SetCanMove(false);
			UpdateUI(item);		}
	}
}