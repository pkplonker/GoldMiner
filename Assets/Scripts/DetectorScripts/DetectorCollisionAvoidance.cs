using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class DetectorCollisionAvoidance : MonoBehaviour
{
	public LayerMask collisionMask;
	private MeshCollider meshCollider;

	private void Awake()
	{
		meshCollider = GetComponent<MeshCollider>();
	}

	public bool CanMove()
	{
		float thresholdDistance = 0.3f;

		Collider[] colliders = Physics.OverlapSphere(transform.position, meshCollider.bounds.extents.magnitude,collisionMask);

		foreach (var collider in colliders)
		{
			if (collider == meshCollider || collider.transform.parent == transform) continue;
			Vector3 closestPoint = collider.ClosestPoint(transform.position);
			float distance = Vector3.Distance(closestPoint, transform.position);

			if (distance < thresholdDistance)
			{
				Debug.Log(collider.name);
				return false;
			}
		}

		return true;
	}
}