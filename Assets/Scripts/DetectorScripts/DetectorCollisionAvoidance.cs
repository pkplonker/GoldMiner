using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DetectorCollisionAvoidance : MonoBehaviour
{
	public LayerMask collisionLayer;

	private BoxCollider boxCollider;

	private void Awake()
	{
		boxCollider = GetComponent<BoxCollider>();
	}

	public bool CanMove()
	{
		Vector3 worldCenter = transform.TransformPoint(boxCollider.center);

		Vector3 halfExtents = Vector3.Scale(boxCollider.size / 2, transform.lossyScale);

		Collider[] hits = Physics.OverlapBox(worldCenter, halfExtents, transform.rotation);

		if (hits.Length > 0)
		{
			foreach (var hit in hits.Where(x => x.gameObject != gameObject && x.transform.parent != transform))
			{
				Debug.Log(hit.name);
				return false;
			}
		}

		// No collisions with other objects were detected
		return true;
	}
}