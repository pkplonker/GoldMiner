//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using Player;
using UnityEngine;

namespace DetectorScripts
{
	/// <summary>
	///DetectorHead full description
	/// </summary>
	public class DetectorHead : MonoBehaviour
	{
		[SerializeField] private float lowerRad = 0.4f;
		[SerializeField] private float upperRad = 0.4f;
		[SerializeField] private float distance = 0.4f;
		private ConeGenerator coneGenerator;
		public static float currentSignal { get; private set; }
		[SerializeField] private float signalDegradeSpeed = 2f;
		public static event Action<float> OnDetection;
		private const string targetLayerMask = "Target";

		private void Start()
		{
			GenerateCone();
		}

		private void GenerateCone()
		{
			GameObject go = new GameObject("Cone");
			go.transform.SetParent(transform);
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.Euler(-90, 0, 0);
			go.transform.localScale = transform.localScale;
			coneGenerator = go.AddComponent<ConeGenerator>();
			coneGenerator.GenerateCone(transform.localPosition, 12, distance, lowerRad, upperRad);
			coneGenerator.enabled = false;
			go.AddComponent<DetectorCone>();

			//debug only
			go.AddComponent<MeshRenderer>();
		}

		private void Update()
		{
			if (!PlayerInteractionStateMachine.isDetecting) return;
			OnDetection?.Invoke(currentSignal);
			DegradeSignal();
		}

		private void DegradeSignal()
		{
			currentSignal = Mathf.Lerp(currentSignal, 0, Time.deltaTime * signalDegradeSpeed);
		}


		private float CalculateSignalStrength(Target t)
		{
			var direction = (t.transform.position - coneGenerator.transform.position).normalized;

			if (!Physics.Raycast(coneGenerator.transform.position, direction, out var hit, distance,
				    LayerMask.GetMask(targetLayerMask))) return 0;

			if (hit.collider.gameObject == t.gameObject)
			{
				Debug.DrawLine(coneGenerator.transform.position, hit.point, Color.red, 1f);
				Debug.Log("distance  = " + hit.distance);
				Debug.Log(1-(hit.distance/distance));
				return 1-(hit.distance/distance);
			}

			return 0;
		}

		public void TargetDetected(Target target)
		{
			if (!PlayerInteractionStateMachine.isDetecting) return;
			currentSignal = CalculateSignalStrength(target);
			Debug.Log("BUZZZZZ");
			OnDetection?.Invoke(currentSignal);
		}
	}
}