using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Player;
using Terrain;
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
			if (Vector3.Distance(stateMachine.transform.position, hit.point) > stateMachine.maxDigRange)
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
			canDig = false;

			AttemptDig();
		}
	}

	private void AttemptDig()
	{
		//cast ray to get vertex
		Vector3 hitPoint = Vector3.negativeInfinity;
		Ray ray = stateMachine.camera.ScreenPointToRay(PlayerInputManager.Instance.GetMousePosition());
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 20f, LayerMask.GetMask(stateMachine.GROUND_LAYER)))
		{
			Debug.Log(Vector3.Distance(stateMachine.transform.position, hit.point));
			hitPoint = hit.point;
		}

		if (hitPoint == Vector3.negativeInfinity)
		{
			Debug.Log("Failed to get hit point");
			return;
		}

		//get mfilter
		var mesh = hit.collider.GetComponent<MeshFilter>().mesh;
		//get closest vertex
		var verts = mesh.vertices.ToList();
		int closest = -1;
		float closestSqrMag = Single.PositiveInfinity;
		float hitSqrMag = hitPoint.sqrMagnitude;
		float currentSqrMag;
		for (int i = 0; i < verts.Count; i++)
		{
			currentSqrMag = Mathf.Abs(hitSqrMag - verts[i].sqrMagnitude) ;
			if (currentSqrMag < closestSqrMag)
			{
				closest = i;
				closestSqrMag = currentSqrMag;
			}
		}

		if (closest == -1)
		{
			Debug.Log("Failed to find closest");
			return;
		}

		//modify vertex
		verts[closest] = new Vector3(verts[closest].x, -stateMachine.digDepth, verts[closest].z);


		//get terrain generator and update vertex array;
		var tg = hit.collider.GetComponent<TerrainGenerator>();
		tg.UpdateMesh(verts);
	}
}