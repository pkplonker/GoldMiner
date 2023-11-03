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
			PlayerInputManager.OnScroll += Scroll;
		}

		protected override void VirtualStateExit()
		{
			stateMachine.Reticle.enabled = false;
			PlayerInputManager.OnScroll -= Scroll;
		}

		public override void Tick()
		{
			var ray = stateMachine.Camera.ScreenPointToRay(ServiceLocator.Instance.GetService<PlayerInputManager>()
				.GetMousePosition());
			if (!Physics.Raycast(ray, out var hit, stateMachine.interactionRange))
			{
				HandleMessage(null);
				return;
			}

			var interactable = hit.collider.GetComponent<IInteractable>();

			HandleMessage(interactable);
			HandleClick(interactable);
		}

		private void Scroll(float scroll)
		{
			if (scroll > 0) stateMachine.ChangeState(stateMachine.DetectingState);
			else  stateMachine.ChangeState(stateMachine.DiggingState);
		}

		private void HandleMessage(IInteractable interactable)
		{
			var notificationBar = ServiceLocator.Instance.GetService<NotificationBar>();
			if (interactable == null) notificationBar.ClearText();
			else notificationBar.RequestText(interactable.GetInteractMessage());
		}

		private void HandleClick(IInteractable interactable)
		{
			if (interactable == null) return;
			if (ServiceLocator.Instance.GetService<PlayerInputManager>().GetLeftClick())
				interactable.Interact(stateMachine);
		}
	}
}