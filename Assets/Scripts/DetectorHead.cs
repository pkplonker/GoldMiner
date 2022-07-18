//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using UnityEngine;

/// <summary>
///DetectorHead full description
/// </summary>
public class DetectorHead : MonoBehaviour
{
	[SerializeField] private float lowerRad = 0.4f;
	[SerializeField] private float upperRad = 0.4f;
	[SerializeField] private float distance = 0.4f;
	private ConeGenerator coneGenerator;
	public static float currentSignal;
	[SerializeField] private float signalDegradeSpeed = 2f;
	public static event Action<float> OnDetection ;

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
		if (!DetectorState.isDetecting) return;
		OnDetection?.Invoke(currentSignal);
		DegradeSignal();
	}

	private void DegradeSignal()
	{
		currentSignal=Mathf.Lerp(currentSignal, 0, Time.deltaTime*signalDegradeSpeed);
	}


	private float CalculateSignalStrength(Target t) => (Vector3.Distance(coneGenerator.transform.position, t.transform.position) /
	                                                    distance) * t.GetSignalStrength();

	public void TargetDetected(Target target)
	{
		if (!DetectorState.isDetecting) return;
		currentSignal = CalculateSignalStrength(target);
		Debug.Log("BUZZZZZ");
		OnDetection?.Invoke(currentSignal);

	}
}