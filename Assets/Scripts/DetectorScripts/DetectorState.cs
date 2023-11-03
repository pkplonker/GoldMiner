//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using Player;
using UnityEngine;

namespace DetectorScripts
{
	/// <summary>
	///DetectionState full description
	/// </summary>
	public class DetectorState : BaseState
	{
		private PlayerInteractionStateMachine stateMachine;
		private static DetectorMovement detectorMovement;
		public static void RegisterDetector(DetectorMovement d) => detectorMovement = d;

		public static void UnregisterDetector(DetectorMovement d)
		{
			if (d == detectorMovement) detectorMovement = null;
			else Debug.Log("Trying to deregister a detector that is not registered");
		}

		private void UpdateHandIK() => stateMachine.RigHandTarget.position = stateMachine.HandleIKTarget.position;

		public override void EnterState(StateMachine sm)
		{
			stateMachine = sm as PlayerInteractionStateMachine;
			stateMachine.Animator.SetLayerWeight(stateMachine.Animator.GetLayerIndex("RightHand"), 1);
			stateMachine.Rig.weight = 1;
			stateMachine.DetectorModel.SetActive(true);
			PlayerInputManager.OnScroll += Scroll;

		}

		protected override void VirtualStateExit()
		{
			stateMachine.Animator.SetLayerWeight(stateMachine.Animator.GetLayerIndex("RightHand"), 0);
			stateMachine.Rig.weight = 0;
			PlayerInteractionStateMachine.IsDetecting = false;
			PlayerInteractionStateMachine.IsManualDetecting = false;
			detectorMovement.ResetPosition();
			stateMachine.DetectorModel.SetActive(false);
			PlayerInputManager.OnScroll -= Scroll;

		}
		private void Scroll(float scroll)
		{
			if (scroll > 0) stateMachine.ChangeState(stateMachine.DiggingState);
			else stateMachine.ChangeState(stateMachine.InteractState);
		}
		public override void Tick()
		{
			UpdateHandIK();
			if (detectorMovement == null) return;
			stateMachine.DetectorModel.SetActive(true);
			if (PlayerInteractionStateMachine.IsManualDetecting) detectorMovement.HandleManualMovement();
			else detectorMovement.HandleAutomaticMovement();
		}
	}
}