using System;
using Player;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.Profiling;
#endif

public class Digging : BaseState
{
	private bool _isDiggingState = false;
	private PlayerInteractionStateMachine _stateMachine;
	private readonly Color _canDigColor = Color.white;
	private readonly Color _cannotDigColor = Color.red;
	private bool _canDig = false;
	private const string NONDIGGABLE_LAYER = "BlocksDig";
	public static Action<Vector3> OnCannotDigHere;
	private void UpdateMarkerPosition()
	{
		Ray ray = _stateMachine.Camera.ScreenPointToRay(PlayerInputManager.Instance.GetMousePosition());
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 20f, LayerMask.GetMask(_stateMachine.GROUND_LAYER)))
		{
			if (Vector3.Distance(_stateMachine.transform.position, hit.point) > _stateMachine.digRange)
			{
				_canDig = false;
				_stateMachine._diggingTarget.color = _cannotDigColor;
			}
			else
			{
				_canDig = true;
				_stateMachine._diggingTarget.color = _canDigColor;
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
			Debug.LogError("invalid state machine");
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
		var ray = _stateMachine.Camera.ScreenPointToRay(PlayerInputManager.Instance.GetMousePosition());
		if (Physics.Raycast(ray, out var hit, 20f, LayerMask.GetMask(GetLayerMask())))
		{
			if (hit.point == Vector3.negativeInfinity) Debug.Log("Failed to get hit point");
			else if (hit.collider != null)
			{
				if (hit.collider.TryGetComponent(out DiggableTerrain terrain))
				{
					if (terrain.Dig(hit, _stateMachine.DigDepth, _stateMachine.MaxDigDepth)) return;
				}
			}
		}

		UnableToDig(hit.point);


#if UNITY_EDITOR
		Profiler.EndSample();
#endif
	}

	private static void UnableToDig(Vector3 position)
	{
		OnCannotDigHere?.Invoke(position);
		Debug.Log("Failed to get diggable terrain");
	}

	private string[] GetLayerMask() => new[] {_stateMachine.GROUND_LAYER, NONDIGGABLE_LAYER};
}