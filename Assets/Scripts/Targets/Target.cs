//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using Player;
using Save;
using UnityEngine;
using UnityEngine.Serialization;

namespace Targets
{
	/// <summary>
	///Target full description
	/// </summary>
	public class Target : MonoBehaviour, IInteractable
	{
		[field: Range(0.01f, 1)]
		[field: SerializeField]
		public float SignalStrength { get; set; } = 0.6f;

		[SerializeField] private bool drawGizmos;
		[SerializeField] private string interactText = "Click to pickup";
		private bool isActiveTarget;
		private static bool drawDebug = true;
		public Vector3 Position => transform.position;

		[FormerlySerializedAs("_gizmoColor")] [SerializeField]
		private Color gizmoColor = Color.red;

		private void Awake() => Register();

		protected virtual void OnDrawGizmos()
		{
			if (drawGizmos) DrawMarker();
		}

		protected virtual void Register() => ServiceLocator.Instance.GetService<TargetManager>().RegisterTarget(this);

		[CheatCommand]
		public static void ToggleDrawDebug() => drawDebug = !drawDebug;

		protected virtual void DrawMarker()
		{
			if (!drawDebug) return;
			SetColor();
			Gizmos.DrawRay(transform.position, Vector3.up);
		}

		protected virtual void SetColor() => Gizmos.color = gizmoColor;
		protected virtual void OnDrawGizmosSelected() => DrawMarker();

		public virtual void Interact(PlayerInteractionStateMachine player) => Debug.Log("Interacted");

		public void DisableObject()
		{
			GetComponent<Collider>().enabled = false;
			GetComponent<MeshRenderer>().enabled = false;
		}

		public string GetInteractMessage() => interactText;

		public void EnableObject()
		{
			GetComponent<Collider>().enabled = true;
			GetComponent<MeshRenderer>().enabled = true;
		}
	}
}