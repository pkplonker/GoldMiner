//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using Player;
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

		[FormerlySerializedAs("_gizmoColor")] [SerializeField]
		private Color gizmoColor = Color.red;

		protected virtual void OnDrawGizmos()
		{
			if (drawGizmos) DrawMarker();
		}

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

		protected void DisableObject()
		{
			GetComponent<Collider>().enabled = false;
			GetComponent<MeshRenderer>().enabled = false;
		}

		public string GetInteractMessage() => interactText;
	}
}