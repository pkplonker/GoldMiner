using System;
using System.Linq;
using UnityEngine;

namespace DetectorScripts
{
	[RequireComponent(typeof(MeshFilter))]
	public class DetectorHeadAlignment : MonoBehaviour
	{
		[SerializeField] private string groundLayer;
		[SerializeField] private float targetGroundOffset = 0.2f;
		[SerializeField] private GameObject shaft;
		[SerializeField] private GameObject handleTarget;
		[SerializeField] private float distanceBehindHandle = 0.2f;
		private float radius;

		private void Awake()
		{
			var bounds = GetComponent<MeshFilter>().mesh.bounds;

			radius = Mathf.Max(bounds.max.x, bounds.max.z);
			Debug.Log(radius);
		}

		RaycastHit[] results = new RaycastHit[10];

		private void Update()
		{
			var ray = new Ray(transform.position + new Vector3(0, 2f, 0), Vector3.down);
			var size = Physics.SphereCastNonAlloc(ray, radius, results);
			for (int i = 0; i < size; i++)
			{
				if (results[i].collider.gameObject.layer != LayerMask.NameToLayer(groundLayer)) continue;
				var targetHeight = results[i].point.y;
				//Debug.Log(targetHeight);
				transform.position = new Vector3(transform.position.x, targetHeight + targetGroundOffset,
					transform.position.z);
				Vector3 adjustedHandlePosition = handleTarget.transform.position -
				                                 (handleTarget.transform.position - transform.position).normalized *
				                                 distanceBehindHandle;
				shaft.transform.position = (adjustedHandlePosition + transform.position) / 2f;
				float distance = Vector3.Distance(adjustedHandlePosition, transform.position);
				shaft.transform.localScale =
					new Vector3(shaft.transform.localScale.x, distance, shaft.transform.localScale.z);
				shaft.transform.up = adjustedHandlePosition - transform.position;
			}
		}

		private void OnCollisionEnter(Collision collision)
		{
			Debug.Log("hit");
		}
	}
}