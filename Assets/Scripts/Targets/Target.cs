//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using Player;
using UnityEngine;

namespace Targets
{
	/// <summary>
	///Target full description
	/// </summary>
	public class Target : MonoBehaviour, IInteractable
	{
		[Range(0.01f, 1)] [SerializeField] private float _signalStrength;
		public float GetSignalStrength() => _signalStrength;
		[SerializeField] private bool _drawGizmos = false;
		[SerializeField] private string _interactText = "Click to pickup";
		private bool _isActiveTarget = false;
		[SerializeField] private Color _gizmoColor = Color.red;
		protected virtual void OnDrawGizmos()
		{
			if (_drawGizmos) DrawMarker();
		}

		protected virtual void DrawMarker()
		{
			SetColor();
			Gizmos.DrawRay(transform.position, Vector3.up);
		}

		protected virtual void SetColor() => Gizmos.color = _gizmoColor;
		protected virtual void OnDrawGizmosSelected() => DrawMarker();

		public virtual void Interact(PlayerInteractionStateMachine player)=>Debug.Log("Interacted");
		
		protected void DisableObject()
		{
			GetComponent<Collider>().enabled = false;
			GetComponent<MeshRenderer>().enabled = false;
		}
		public string GetInteractMessage() => _interactText;
	}
}