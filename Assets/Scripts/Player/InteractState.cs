using UnityEditor;
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
			var ray = _stateMachine.Camera.ScreenPointToRay(PlayerInputManager.Instance.GetMousePosition());
			if (!Physics.Raycast(ray, out var hit, _stateMachine.interactionRange))
			{
				HandleMessage(null);
				return;
			}
			var interactable = hit.collider.GetComponentInChildren<IInteractable>();
			
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
			if (PlayerInputManager.Instance.GetLeftClick()) interactable.Interact(_stateMachine);
			
			
		}
	}
}