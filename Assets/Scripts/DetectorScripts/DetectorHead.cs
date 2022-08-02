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
		[SerializeField] private float _lowerRad = 0.4f;
		[SerializeField] private float _upperRad = 0.4f;
		[SerializeField] private float _distance = 0.4f;
		private ConeGenerator _coneGenerator;
		public static float CurrentSignal { get; private set; }
		[SerializeField] private float _signalDegradeSpeed = 2f;
		public static event Action<float> OnDetection;
		private const string TARGET_LAYER_MASK = "Target";

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
			_coneGenerator = go.AddComponent<ConeGenerator>();
			_coneGenerator.GenerateCone( 12, _distance, _lowerRad, _upperRad);
			_coneGenerator.enabled = false;
			go.AddComponent<DetectorCone>();

			//debug only
			go.AddComponent<MeshRenderer>();
		}

		private void Update()
		{
			if (!PlayerInteractionStateMachine.IsDetecting) return;
			OnDetection?.Invoke(CurrentSignal);
			DegradeSignal();
		}

		private void DegradeSignal()
		{
			CurrentSignal = Mathf.Lerp(CurrentSignal, 0, Time.deltaTime * _signalDegradeSpeed);
		}


		private float CalculateSignalStrength(Target t)
		{
			var direction = (t.transform.position - _coneGenerator.transform.position).normalized;

			if (!Physics.Raycast(_coneGenerator.transform.position, direction, out var hit, _distance,
				    LayerMask.GetMask(TARGET_LAYER_MASK))) return 0;

			if (hit.collider.gameObject != t.gameObject) return 0;
			Debug.DrawLine(_coneGenerator.transform.position, hit.point, Color.red, 1f);
			Debug.Log("distance  = " + hit.distance);
			Debug.Log(1-(hit.distance/_distance));
			return 1-(hit.distance/_distance);

		}

		public void TargetDetected(Target target)
		{
			if (!PlayerInteractionStateMachine.IsDetecting) return;
			CurrentSignal = CalculateSignalStrength(target);
			Debug.Log("BUZZZZZ");
			OnDetection?.Invoke(CurrentSignal);
		}
	}
}