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
	[SerializeField] private float headRadius = 0.4f;
	[SerializeField] private float distance = 0.4f;
	public static event Action<float> OnDetection;

	private void Update()
	{
		Physics.SphereCast(transform.position, headRadius, Vector3.down, out var raycastHit, distance);
		if (raycastHit.collider == null) return;
		if (raycastHit.collider.TryGetComponent<Target>(out var t) == false)
		{
			OnDetection?.Invoke(0);

			return;
		}

		Debug.Log("Buzzzzz");
		var x = CalculateSignalStrength(t);
		Debug.Log(x);
		OnDetection?.Invoke(x);
	}

	private float CalculateSignalStrength(Target t) => (Vector3.Distance(transform.position, t.transform.position) /
	                                                    distance) * t.GetSignalStrength();


	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(transform.position, headRadius);
		Gizmos.DrawSphere(transform.position - (Vector3.down * distance), headRadius);
	}
}