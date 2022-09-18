using System;
using Player;
using UI;
using UnityEngine;

namespace Targets
{
	public class PickupTarget : Target
	{
		[SerializeField] private Item _item;
		public Item GetItem() => _item;

	

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

			CanvasGroupController.Instance.ShowNewItemPickupUI(this, player);
		}

		public void DestroyItem()
		{
			Destroy(gameObject);
		}
	}
}