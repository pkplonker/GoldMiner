using System;
using Player;
using UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Targets
{
	public class PickupTarget : Target
	{
		[field: SerializeField] public Item Item { get; private set; }
		private NewItemPickupUI newItemPickupUI;

		public override void Interact(PlayerInteractionStateMachine player)
		{
			if (player == null)
			{
				Debug.LogError("Player is null");
				return;
			}

			if (player.GetComponent<Inventory>() == null)
			{
				Debug.LogError("Player invent is null");
				return;
			}

			CanvasGroupController.Instance.Show(CanvasGroupController.Instance.NewItemPickupUI);
			CanvasGroupController.Instance.NewItemPickupUI.Init(this, player);
		}

		public void DestroyItem()
		{
			Destroy(gameObject);
		}
	}
}