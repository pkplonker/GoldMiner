using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Player;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.Profiling;
#endif

public class Digging : BaseState
{
	private bool _isDiggingState = false;
	private new PlayerInteractionStateMachine _stateMachine;
	private readonly Color canDigColor = Color.white;
	private readonly Color cannotDigColor = Color.red;
	private bool _canDig = false;

	private void UpdateMarkerPosition()
	{
		Ray ray = _stateMachine.Camera.ScreenPointToRay(PlayerInputManager.Instance.GetMousePosition());
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 20f, LayerMask.GetMask(_stateMachine.GROUND_LAYER)))
		{
			if (Vector3.Distance(_stateMachine.transform.position, hit.point) > _stateMachine._maxDigRange)
			{
				_canDig = false;
				_stateMachine._diggingTarget.color = cannotDigColor;
			}
			else
			{
				_canDig = true;
				_stateMachine._diggingTarget.color = canDigColor;
			}

			_stateMachine._diggingTarget.transform.position = hit.point;
			_stateMachine._diggingTarget.transform.position += (hit.normal * 0.1f);
			_stateMachine._diggingTarget.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
		}
	}

	private void ToggleDigging()
	{
		_isDiggingState = !_isDiggingState;
	}

	public override void EnterState(StateMachine sm)
	{
		_canDig = false;
		_stateMachine = sm as PlayerInteractionStateMachine;
		if (_stateMachine == null)
		{
			Debug.LogError("in valid state machine");
		}

		_isDiggingState = true;
		_stateMachine._diggingTarget.enabled = true;
		PlayerInputManager.OnDiggingToggle += ToggleDigging;
	}


	protected override void VirtualStateExit()
	{
		_canDig = false;
		_isDiggingState = false;
		_stateMachine._diggingTarget.enabled = false;
		PlayerInputManager.OnDiggingToggle -= ToggleDigging;
	}

	public override void Tick()
	{
		UpdateMarkerPosition();
		if (PlayerInputManager.Instance.GetLeftClick() && _canDig)
		{
			_canDig = false;

			AttemptDig();
		}
	}

	private void AttemptDig()
	{
#if UNITY_EDITOR

		Profiler.BeginSample("Digging");
#endif
		//cast ray to get vertex
		Ray ray = _stateMachine.Camera.ScreenPointToRay(PlayerInputManager.Instance.GetMousePosition());
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 20f, LayerMask.GetMask(_stateMachine.GROUND_LAYER)))
		{
		}

		if (hit.point == Vector3.negativeInfinity)
		{
			Debug.Log("Failed to get hit point");
			return;
		}
		
		if (hit.collider != null)
		{
			Debug.Log($"Triggering hit at {hit.point} ");
			//get terrain generator and update vertex array;
			var tg = hit.collider.GetComponent<DiggableTerrain>();
			if (tg != null) tg.Dig(hit, _stateMachine._digDepth);
			else Debug.Log("Failed to get diggable terrain");
		}
		

#if UNITY_EDITOR
		Profiler.EndSample();
#endif
	}
}