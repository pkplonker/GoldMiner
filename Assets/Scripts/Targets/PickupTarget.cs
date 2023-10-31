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

			var canvas = ServiceLocator.Instance.GetService<CanvasGroupController>();
			canvas.Show(canvas.NewItemPickupUI);
			canvas.NewItemPickupUI.Init(this, player);
		}
	}
}