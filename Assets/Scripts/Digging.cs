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
			Debug.Log("Dig dig" + Time.frameCount);
			canDig = false;

			AttemptDig();
		}
	}

	private void AttemptDig()
	{
#if UNITY_EDITOR

		Profiler.BeginSample("Digging");
#endif
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

		var index = hit.triangleIndex;
		List<Vector3> hitVerts = new List<Vector3>();
		hitVerts.Add(mesh.vertices[mesh.triangles[index * 3 + 0]]);
		hitVerts.Add(mesh.vertices[mesh.triangles[index * 3 + 1]]);
		hitVerts.Add(mesh.vertices[mesh.triangles[index * 3 + 2]]);


		var verts = mesh.vertices.ToList();

		for (var i = 0; i < verts.Count; i++)
		{
			var v = verts[i];
			if (hitVerts.Any(hv => v == hv))
			{
				verts[i] = new Vector3(v.x, v.y - stateMachine.digDepth, v.z);
			}
		}

		//get terrain generator and update vertex array;
		//var tg = hit.collider.GetComponent<DiggableTerrain>();
		//if (tg != null) tg.UpdateMesh(verts);
		//else Debug.Log("Failed to get diggable terrain");

#if UNITY_EDITOR
		Profiler.EndSample();
#endif
	}

	private void SpawnTestBall(Vector3 hitPoint, string name)
	{
		var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		go.transform.localScale = Vector3.one * 0.1f;
		go.transform.SetPositionAndRotation(hitPoint, Quaternion.identity);
		go.name = name;
	}
}