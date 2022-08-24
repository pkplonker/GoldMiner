using Player;
using UnityEngine;

namespace Targets
{
	public class ValuableTarget : Target
	{
		[SerializeField] private Item _item;

		public override bool Interact(PlayerInteractionStateMachine player)
		{
			if (player.GetComponent<Inventory>().Add(_item))
			{
				DisableObject();
				return true;
			}

			return false;

		}
		
	}
}