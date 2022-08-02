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
			Debug.Log("Dig dig" + Time.frameCount);
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
		Vector3 hitPoint = Vector3.negativeInfinity;
		Ray ray = _stateMachine.Camera.ScreenPointToRay(PlayerInputManager.Instance.GetMousePosition());
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 20f, LayerMask.GetMask(_stateMachine.GROUND_LAYER)))
		{
			Debug.Log(Vector3.Distance(_stateMachine.transform.position, hit.point));
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
				verts[i] = new Vector3(v.x, v.y - _stateMachine._digDepth, v.z);
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