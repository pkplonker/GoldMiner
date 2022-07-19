using UnityEngine;

namespace DetectorScripts
{
	[RequireComponent(typeof(Rigidbody), typeof(MeshCollider))]
	public class DetectorCone : MonoBehaviour
	{
		private DetectorHead detectorHead;

		private void Awake()
		{
			var mc = GetComponent<MeshCollider>();
			mc.convex = true;
			mc.isTrigger = true;
			GetComponent<Rigidbody>().isKinematic = true;
			detectorHead = GetComponentInParent<DetectorHead>();
			if (detectorHead == null) Debug.LogError("detector head not detected");
		}

		private void OnTriggerEnter(Collider other)
		{
			HandleTrigger(other);
		}

		private void OnTriggerStay(Collider other)
		{
			HandleTrigger(other);
		}

		private void HandleTrigger(Collider other)
		{
			if (other.TryGetComponent(typeof(Target), out var target))
			{
				detectorHead.TargetDetected(target as Target);
			}
		}
	}
}