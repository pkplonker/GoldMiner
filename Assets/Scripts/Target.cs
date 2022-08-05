//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using Player;
using UnityEditor;
using UnityEngine;

/// <summary>
///Target full description
/// </summary>
public class Target : MonoBehaviour, IInteractable, IHighlightable
{
	[Range(0.01f, 1)] [SerializeField] private float _signalStrength;
	public float GetSignalStrength() => _signalStrength;
	[SerializeField] private bool drawGizmos = false;


	protected virtual void OnDrawGizmos()
	{
		if (drawGizmos) DrawMarker();
	}

	protected virtual void DrawMarker()
	{
		SetColor();
		Gizmos.DrawRay(transform.position, Vector3.up);
	}

	protected virtual void SetColor() => Gizmos.color = Color.red;
	protected virtual void OnDrawGizmosSelected() => DrawMarker();

	public virtual bool Interact(PlayerInteractionStateMachine player)
	{
		Debug.Log("Interacted");
		return true;
	}

	public virtual void Highlight(PlayerInteractionStateMachine player)
	{
		Debug.Log("Highlight");
	}
}