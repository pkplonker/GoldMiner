//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using Player;
using Targets;
using UnityEngine;
using UnityEngine.Serialization;

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
		[SerializeField] private float lowerDistanceThreshHoldForMaxOutput = 0.3f;
		[SerializeField] private string coneLayer = "DetectorCone";
		private ConeGenerator coneGenerator;
		public static float CurrentSignal { get; private set; }
		[SerializeField] private float signalDegradeSpeed = 2f;
		public static event Action<float> OnDetection;
		private const string TARGET_LAYER_MASK = "StaticProp";

		private void Start()
		{
			GenerateCone();
			CurrentSignal = 0;
			OnDetection?.Invoke(CurrentSignal);
		}

		private void GenerateCone()
		{
			var go = new GameObject("Cone");
			go.transform.SetParent(transform);
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.Euler(-90, 0, 0);
			go.transform.localScale = transform.localScale;
			coneGenerator = go.AddComponent<ConeGenerator>();
			coneGenerator.GenerateCone(12, distance, lowerRad, upperRad);
			coneGenerator.enabled = false;
			go.AddComponent<DetectorCone>();
			go.layer = LayerMask.NameToLayer(coneLayer);
#if UNITY_EDITOR
			go.AddComponent<MeshRenderer>();
#endif
		}

		private void Update()
		{
			if (!PlayerInteractionStateMachine.IsDetecting) return;
			OnDetection?.Invoke(CurrentSignal);
			DegradeSignal();
		}

		private void DegradeSignal()
		{
			CurrentSignal = Mathf.Lerp(CurrentSignal, 0, Time.deltaTime * signalDegradeSpeed);
		}

		private float CalculateSignalStrength(Target t)
		{
			var direction = (t.transform.position - coneGenerator.transform.position).normalized;

			if (!Physics.Raycast(coneGenerator.transform.position, direction, out var hit, distance,
				    LayerMask.GetMask(TARGET_LAYER_MASK))) return 0;

			if (hit.collider.gameObject != t.gameObject) return 0;
			Debug.DrawLine(coneGenerator.transform.position, hit.point, Color.red, 1f);
			//	Debug.Log("distance  = " + hit.distance);
			return CalculateStrength(hit);
		}

		private float CalculateStrength(RaycastHit hit)
		{
			var val = Mathf.Clamp01(1 - (hit.distance / distance));
			//Debug.Log(val);
			if (val <= lowerDistanceThreshHoldForMaxOutput)
				return 0;
			return val;
		}

		public void TargetDetected(Target target)
		{
			if (!PlayerInteractionStateMachine.IsDetecting) return;
			CurrentSignal = CalculateSignalStrength(target);
			//Debug.Log($"Signal Strength: {CurrentSignal}");
			OnDetection?.Invoke(CurrentSignal);
		}
	}
}