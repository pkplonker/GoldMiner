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
		private new PlayerInteractionStateMachine stateMachine;
		private static DetectorMovement detectorMovement;

		public static void RegisterDetector(DetectorMovement d) => detectorMovement = d;


		public static void UnregisterDetector(DetectorMovement d)
		{
			if (d == detectorMovement) detectorMovement = null;
			else Debug.Log("Trying to deregister a detector that is not registered");
		}


		private void UpdateHandIK()
		{
			stateMachine.rigHandTarget.position = stateMachine.handleIKTarget.position;
		}

		public override void EnterState(StateMachine sm)
		{
			stateMachine = sm as PlayerInteractionStateMachine;
			stateMachine.animator.SetLayerWeight(stateMachine.animator.GetLayerIndex("RightHand"), 1);
			stateMachine.rig.weight = 1;
			stateMachine.detectorModel.SetActive(true);

		}

		protected override void VirtualStateExit()
		{
			stateMachine.animator.SetLayerWeight(stateMachine.animator.GetLayerIndex("RightHand"), 0);
			stateMachine.rig.weight = 0;
			PlayerInteractionStateMachine.isDetecting = false;
			PlayerInteractionStateMachine.isManualDetecting = false;
			detectorMovement.ResetPosition();
			stateMachine.detectorModel.SetActive(false);
		}

		public override void Tick()
		{
			UpdateHandIK();
			if (detectorMovement == null) return;
			stateMachine.detectorModel.SetActive(true);
			if (PlayerInteractionStateMachine.isManualDetecting) detectorMovement.HandleManualMovement();
			else detectorMovement.HandleAutomaticMovement();
		}
	}
}