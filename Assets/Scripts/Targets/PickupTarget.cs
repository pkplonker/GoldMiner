using System;
using Player;
using UI;
using UnityEngine;

namespace Targets
{
	public class PickupTarget : Target
	{
		[SerializeField] private Item _item;
		[SerializeField] private NewItemPickupUI _newItemPickupUIPrefab;
		private NewItemPickupUI _newItemPickupUI;
		public Item GetItem() => _item;

		private void OnValidate()
		{
			if (_newItemPickupUIPrefab == null) Debug.Log("Missing UI");
		}

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

			_newItemPickupUI = Instantiate(_newItemPickupUIPrefab);
			_newItemPickupUI.Show(this,player);
		}

		public void DestroyItem()
		{
			Destroy(gameObject);
		}
	}
}