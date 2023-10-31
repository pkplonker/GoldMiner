using System;
using System.Globalization;
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
		[SerializeField] private TextMeshProUGUI itemNameText;
		[SerializeField] private TextMeshProUGUI valueText;
		[SerializeField] private Image itemImage;
		private PickupTarget pickupTarget;
		private PlayerInteractionStateMachine playerInteractionStateMachine;
		private PlayerMovement playerMovement;
		[SerializeField] private Button keepButton;

		private void UpdateUI(Item item)
		{
			itemNameText.text = item.ItemName;
			itemImage.sprite = item.Sprite;
			valueText.text = item.GetValue().ToString(CultureInfo.InvariantCulture);
		}

		//ui
		public void Pickup()
		{
			if (pickupTarget == null || playerInteractionStateMachine == null) Debug.LogError("missing refs");

			var inv = playerInteractionStateMachine.GetComponent<Inventory>();
			if (inv == null || !inv.Add(pickupTarget.Item))
			{
				FailedToAddToInventory();
			}
			else
			{
				ServiceLocator.Instance.GetService<TargetManager>()?.DeregisterTarget(pickupTarget);
			}

			ServiceLocator.Instance.GetService<CanvasGroupController>().Hide(this);
		}

		private void FailedToAddToInventory() => Debug.LogError("Failed to add to inventory");

		//ui
		public void ThrowAway()
		{
			if (pickupTarget == null) Debug.LogError("missing refs");
			ServiceLocator.Instance.GetService<TargetManager>()?.DeregisterTarget(pickupTarget);
			ServiceLocator.Instance.GetService<CanvasGroupController>().Hide(this);
		}

		public override void Toggle()
		{
			base.Toggle();
			EventSystem.current.SetSelectedGameObject(keepButton.gameObject);
		}

		public override void Hide()
		{
			base.Hide();
			playerMovement.SetCanMove(true);
		}

		public void Init(PickupTarget pickupTarget, PlayerInteractionStateMachine player)
		{
			playerInteractionStateMachine = player;
			this.pickupTarget = pickupTarget;
			var item = this.pickupTarget.Item;
			if (item == null)
			{
				Debug.LogError("Item is null");
				return;
			}

			playerMovement = playerInteractionStateMachine.GetComponent<PlayerMovement>();
			if (playerMovement == null)
			{
				Debug.LogError("Cannot inhibit player movement");
				return;
			}

			playerMovement.SetCanMove(false);
			UpdateUI(item);
		}
	}
}