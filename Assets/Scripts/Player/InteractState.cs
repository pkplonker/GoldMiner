using UnityEngine;
namespace Player
{
	public class InteractState : BaseState
	{
		private PlayerInteractionStateMachine _stateMachine;

		public override void EnterState(StateMachine sm)
		{
			_stateMachine = sm as PlayerInteractionStateMachine;
			_stateMachine.Reticle.enabled = true;
		}

		protected override void VirtualStateExit() => _stateMachine.Reticle.enabled = false;

		public override void Tick()
		{
			//cast ray to get vertex
			var ray = _stateMachine.Camera.ScreenPointToRay(PlayerInputManager.Instance.GetMousePosition());
			if (Physics.Raycast(ray, out var hit, _stateMachine.interactionRange))
			{
				var interactable = hit.collider.GetComponent<IInteractable>();
				if (interactable!=null)
				{
					if (PlayerInputManager.Instance.GetLeftClick())
					{
						if (interactable.Interact(_stateMachine))
						{
							Debug.Log("Interaction successful");
						}
						else
						{
							Debug.Log("Failed interaction");
						}
					}
				}
				
			}
		}
	}
}