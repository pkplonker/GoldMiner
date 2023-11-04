using System;
using System.Linq;
using UnityEngine;

namespace DetectorScripts
{
	public class DetectorHeadAlignment : MonoBehaviour
	{
		[SerializeField] private string groundLayer;
		[SerializeField] private GameObject detector;
		[SerializeField] private float targetGroundOffset = 0.3f;
		[SerializeField] private Vector3 groundDetectionStartingHeight;
		[SerializeField] private float maxMove = 0.3f;

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
			Debug.Log("test");
			Debug.DrawLine(transform.position + new Vector3(0, 2f, 0), transform.position - new Vector3(0, 2f, 0),
				Color.cyan, .2f);
			var results = Physics.RaycastAll(transform.position + new Vector3(0, 2f, 0), Vector3.down, 4f);

			foreach (var result in results)
			{
				if (result.collider.gameObject.layer == LayerMask.NameToLayer(groundLayer))
				{
					Debug.Log("setting position");
					transform.position =
						new Vector3(transform.position.x, result.point.y+0.2f, transform.position.z);
				}
			}

			//hit.point.y + targetGroundOffset
		}

		private void OnCollisionEnter(Collision collision)
		{
			Debug.Log("hit");
		}
	}
}