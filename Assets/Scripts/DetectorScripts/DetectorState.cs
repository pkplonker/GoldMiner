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
		private new PlayerInteractionStateMachine _stateMachine;
		private static DetectorMovement _detectorMovement;

		public static void RegisterDetector(DetectorMovement d) => _detectorMovement = d;


		public static void UnregisterDetector(DetectorMovement d)
		{
			if (d == _detectorMovement) _detectorMovement = null;
			else Debug.Log("Trying to deregister a detector that is not registered");
		}


		private void UpdateHandIK()
		{
			_stateMachine._rigHandTarget.position = _stateMachine._handleIKTarget.position;
		}

		public override void EnterState(StateMachine sm)
		{
			_stateMachine = sm as PlayerInteractionStateMachine;
			_stateMachine._animator.SetLayerWeight(_stateMachine._animator.GetLayerIndex("RightHand"), 1);
			_stateMachine._rig.weight = 1;
			_stateMachine._detectorModel.SetActive(true);

		}

		protected override void VirtualStateExit()
		{
			_stateMachine._animator.SetLayerWeight(_stateMachine._animator.GetLayerIndex("RightHand"), 0);
			_stateMachine._rig.weight = 0;
			PlayerInteractionStateMachine.IsDetecting = false;
			PlayerInteractionStateMachine.IsManualDetecting = false;
			_detectorMovement.ResetPosition();
			_stateMachine._detectorModel.SetActive(false);
		}

		public override void Tick()
		{
			UpdateHandIK();
			if (_detectorMovement == null) return;
			_stateMachine._detectorModel.SetActive(true);
			if (PlayerInteractionStateMachine.IsManualDetecting) _detectorMovement.HandleManualMovement();
			else _detectorMovement.HandleAutomaticMovement();
		}
	}
}