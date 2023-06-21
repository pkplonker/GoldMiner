using UnityEditor;
using UnityEngine;

namespace Player
{
	public class InteractState : BaseState
	{
		private PlayerInteractionStateMachine stateMachine;

		public override void EnterState(StateMachine sm)
		{
			stateMachine = sm as PlayerInteractionStateMachine;
			stateMachine.Reticle.enabled = true;
		}

		protected override void VirtualStateExit() => stateMachine.Reticle.enabled = false;

		public override void Tick()
		{
			var ray = stateMachine.Camera.ScreenPointToRay(PlayerInputManager.Instance.GetMousePosition());
			if (!Physics.Raycast(ray, out var hit, stateMachine.interactionRange))
			{
				HandleMessage(null);
				return;
			}
			var interactable = hit.collider.GetComponent<IInteractable>();
			
			HandleMessage(interactable);
			HandleClick(interactable);
		}

		private void HandleMessage(IInteractable interactable)
		{
			if (interactable == null) NotificationBar.Instance.ClearText();
			else NotificationBar.Instance.RequestText(interactable.GetInteractMessage());

		}

		private void HandleClick(IInteractable interactable)
		{
			if (interactable == null) return;
			if (PlayerInputManager.Instance.GetLeftClick()) interactable.Interact(stateMachine);
			
			
		}
	}
}