using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class Digging : BaseState
{
	private bool isDiggingState = false;
	private new PlayerInteractionStateMachine stateMachine;

	private void UpdateMarkerPosition()
	{
		Ray ray = stateMachine.camera.ScreenPointToRay(PlayerInputManager.Instance.GetMousePosition());
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 20f, LayerMask.GetMask(stateMachine.GROUND_LAYER)))
		{
			stateMachine.diggingTarget.transform.position = hit.point;
			stateMachine.diggingTarget.transform.position += (hit.normal * 0.1f);
			stateMachine.diggingTarget.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
		}
	}

	private void ToggleDigging()
	{
		isDiggingState = !isDiggingState;
	}

	public override void EnterState(StateMachine sm)
	{
		stateMachine = sm as PlayerInteractionStateMachine;
		if (stateMachine == null)
		{
			Debug.LogError("in valid state machine");
		}

		isDiggingState = true;
		stateMachine.diggingTarget.enabled = isDiggingState;
		PlayerInputManager.OnDiggingToggle += ToggleDigging;
	}


	protected override void VirtualStateExit()
	{
		isDiggingState = false;
		stateMachine.diggingTarget.enabled = isDiggingState;
		PlayerInputManager.OnDiggingToggle -= ToggleDigging;
	}

	public override void Tick()
	{
		UpdateMarkerPosition();
	}
}