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
		private  PlayerInteractionStateMachine _stateMachine;
		private static DetectorMovement _detectorMovement;
		public static void RegisterDetector(DetectorMovement d) => _detectorMovement = d;


		public static void UnregisterDetector(DetectorMovement d)
		{
			if (d == _detectorMovement) _detectorMovement = null;
			else Debug.Log("Trying to deregister a detector that is not registered");
		}


		private void UpdateHandIK()=>_stateMachine.RigHandTarget.position = _stateMachine.HandleIKTarget.position;
		

		public override void EnterState(StateMachine sm)
		{
			_stateMachine = sm as PlayerInteractionStateMachine;
			_stateMachine.Animator.SetLayerWeight(_stateMachine.Animator.GetLayerIndex("RightHand"), 1);
			_stateMachine.Rig.weight = 1;
			_stateMachine.DetectorModel.SetActive(true);

		}

		protected override void VirtualStateExit()
		{
			_stateMachine.Animator.SetLayerWeight(_stateMachine.Animator.GetLayerIndex("RightHand"), 0);
			_stateMachine.Rig.weight = 0;
			PlayerInteractionStateMachine.IsDetecting = false;
			PlayerInteractionStateMachine.IsManualDetecting = false;
			_detectorMovement.ResetPosition();
			_stateMachine.DetectorModel.SetActive(false);

		}

		public override void Tick()
		{
			UpdateHandIK();
			if (_detectorMovement == null) return;
			_stateMachine.DetectorModel.SetActive(true);
			if (PlayerInteractionStateMachine.IsManualDetecting) _detectorMovement.HandleManualMovement();
			else _detectorMovement.HandleAutomaticMovement();
		}
	}
}