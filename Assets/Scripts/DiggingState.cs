using System;
using Player;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.Profiling;
#endif

public class DiggingState : BaseState
{
	private bool isDiggingState = false;
	private PlayerInteractionStateMachine stateMachine;
	private readonly Color canDigColor = Color.white;
	private readonly Color cannotDigColor = Color.red;
	private bool canDig;
	private const string NONDIGGABLE_LAYER = "BlocksDig";
	public static Action<Vector3> OnCannotDigHere;

	private void UpdateMarkerPosition()
	{
		stateMachine.diggingTarget.enabled = true;
		;
		var ray = stateMachine.Camera.ScreenPointToRay(ServiceLocator.Instance.GetService<PlayerInputManager>()
			.GetMousePosition());
		if (!Physics.Raycast(ray, out var hit, 20f, LayerMask.GetMask(stateMachine.GROUND_LAYER))) return;
		if (Vector3.Distance(stateMachine.transform.position, hit.point) > stateMachine.digRange)
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

	private void ToggleDigging() => isDiggingState = !isDiggingState;

	public override void EnterState(StateMachine sm)
	{
		canDig = false;
		stateMachine = sm as PlayerInteractionStateMachine;
		if (stateMachine == null)
		{
			Debug.LogError("invalid state machine");
			return;
		}

		isDiggingState = true;
		stateMachine.diggingTarget.enabled = true;
		PlayerInputManager.OnDiggingToggle += ToggleDigging;
		PlayerInputManager.OnScroll += Scroll;

	}

	private void Scroll(float scroll)
	{
		if(scroll>0) stateMachine.ChangeState(stateMachine.InteractState);
		else stateMachine.ChangeState(stateMachine.DetectingState);
	}

	protected override void VirtualStateExit()
	{
		canDig = false;
		isDiggingState = false;
		stateMachine.diggingTarget.enabled = false;
		PlayerInputManager.OnDiggingToggle -= ToggleDigging;
		PlayerInputManager.OnScroll -= Scroll;

	}

	public override void Tick()
	{
		UpdateMarkerPosition();
		var player = ServiceLocator.Instance.GetService<PlayerInputManager>();
		if ((!player.GetLeftClick() && !player.GetPanRightHeld()) || !canDig) return;
		canDig = false;
		AttemptDig();
	}

	private void AttemptDig()
	{
#if UNITY_EDITOR

		Profiler.BeginSample("Digging");
#endif
		//cast ray to get vertex
		var ray = stateMachine.Camera.ScreenPointToRay(ServiceLocator.Instance.GetService<PlayerInputManager>()
			.GetMousePosition());
		if (Physics.Raycast(ray, out var hit, 20f, LayerMask.GetMask(GetLayerMask())))
		{
			if (hit.point == Vector3.negativeInfinity) Debug.Log("Failed to get hit point");
			else if (hit.collider != null)
			{
				if (hit.collider.TryGetComponent(out DiggableTerrain terrain))
				{
					if (terrain.Dig(hit,
						    new DiggableTerrain.DigParams
							    {DigAmount = stateMachine.DigDepth, PlayVFX = true})) return;
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

	private string[] GetLayerMask() => new[] {stateMachine.GROUND_LAYER, NONDIGGABLE_LAYER};
}