using System;
using System.Linq;
using UnityEngine;

namespace DetectorScripts
{
	public class DetectorHeadAlignment : MonoBehaviour
	{
		[SerializeField] private string groundLayer;
		[SerializeField] private float targetGroundOffset = 0.2f;
		[SerializeField] private GameObject shaft;
		[SerializeField] private GameObject handleTarget;
		[SerializeField] private float distanceBehindHandle = 0.2f;

		private void Update()
		{
			 
			var results = Physics.RaycastAll(transform.position + new Vector3(0, 2f, 0), Vector3.down, 4f);

			foreach (var result in results)
			{
				if (result.collider.gameObject.layer != LayerMask.NameToLayer(groundLayer)) continue;
				transform.position = new Vector3(transform.position.x, result.point.y + targetGroundOffset, transform.position.z);
				Vector3 adjustedHandlePosition = handleTarget.transform.position - (handleTarget.transform.position - transform.position).normalized * distanceBehindHandle;
				shaft.transform.position = (adjustedHandlePosition + transform.position) / 2f;
				float distance = Vector3.Distance(adjustedHandlePosition, transform.position);
				shaft.transform.localScale = new Vector3(shaft.transform.localScale.x, distance, shaft.transform.localScale.z);
				shaft.transform.up = adjustedHandlePosition - transform.position;
			}
		}

		private void OnCollisionEnter(Collision collision)
		{
			Debug.Log("hit");
		}
	}
}