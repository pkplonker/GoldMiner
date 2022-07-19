using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class Digging : BaseState
{
	private bool isDiggingState = false;
	private new PlayerInteractionStateMachine stateMachine;
	private Color canDigColor = Color.white;
	private Color cannotDigColor = Color.red;
	private bool canDig = false;

	private void UpdateMarkerPosition()
	{
		Ray ray = stateMachine.camera.ScreenPointToRay(PlayerInputManager.Instance.GetMousePosition());
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 20f, LayerMask.GetMask(stateMachine.GROUND_LAYER)))
		{
			Debug.Log(Vector3.Distance(stateMachine.transform.position,hit.point) );
			if (Vector3.Distance(stateMachine.transform.position,hit.point) > stateMachine.maxDigRange)
			{
				canDig = false;
				stateMachine.diggingTarget.color = cannotDigColor;
			}
			else
			{
				canDig = true;
				stateMachine.diggingTarget.color = canDigColor;
			}

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
		canDig = false;
		stateMachine = sm as PlayerInteractionStateMachine;
		if (stateMachine == null)
		{
			Debug.LogError("in valid state machine");
		}

		isDiggingState = true;
		stateMachine.diggingTarget.enabled = true;
		PlayerInputManager.OnDiggingToggle += ToggleDigging;
	}


	protected override void VirtualStateExit()
	{
		canDig = false;
		isDiggingState = false;
		stateMachine.diggingTarget.enabled = false;
		PlayerInputManager.OnDiggingToggle -= ToggleDigging;
	}

	public override void Tick()
	{
		UpdateMarkerPosition();
		if (PlayerInputManager.Instance.GetLeftClick() && canDig)
		{
			Debug.Log("Dig dig");
		}
	}
}