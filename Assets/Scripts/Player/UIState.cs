//
// Copyright (C) 2023 Stuart Heath. All rights reserved.
//

/// <summary>
///UIState full description
/// </summary>

namespace Player
{
	public class UIState : BaseState
	{
		private PlayerInteractionStateMachine stateMachine;

		public override void EnterState(StateMachine sm)
		{
			stateMachine = sm as PlayerInteractionStateMachine;
			stateMachine.Reticle.enabled = false;
		}

		protected override void VirtualStateExit()
		{
			
		}

		public override void Tick()
		{
			if(stateMachine.CanMove) stateMachine.ChangeState(stateMachine.PreviousState);
		}
	}
}