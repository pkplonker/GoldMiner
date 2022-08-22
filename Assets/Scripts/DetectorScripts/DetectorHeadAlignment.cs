using System;
using UnityEngine;

namespace DetectorScripts
{
	public class DetectorHeadAlignment : MonoBehaviour
	{
		[SerializeField] private string groundLayer;
		[SerializeField] private GameObject detector;
		[SerializeField] private float targetGroundOffset = 0.3f;
		[SerializeField] private Vector3 groundDetectionStartingHeight;
		private Vector3 startPosition;
		private void Awake()
		{
			enabled = false;
		}

		private void OnEnable()
		{
			startPosition = detector.transform.position;

		}

		private void Update()
		{
			
		}

		private void OnCollisionEnter(Collision collision)
		{
			Debug.Log("hit");
		}
	}
}
