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
		if (!DetectorState.isDetecting) return;
		
		Physics.SphereCast(transform.position, headRadius, Vector3.down, out var raycastHit, distance);
		Debug.DrawRay(transform.position,Vector3.down * distance, Color.red);
		DrawDebug();
		if (raycastHit.collider == null || raycastHit.collider.TryGetComponent<Target>(out var t) == false)
		{
			OnDetection?.Invoke(0);
			return;
		}

		Debug.Log("Buzzzzz");
		var x = CalculateSignalStrength(t);
		Debug.Log(x);
		OnDetection?.Invoke(x);
	}

	private void DrawDebug()
	{
		for (var i = 0; i < 360; i++)
		{
			var angle = i * Mathf.Deg2Rad;
			var x = Mathf.Cos(angle);
			var z = Mathf.Sin(angle);
			var point = new Vector3(x,0, z);
			Debug.DrawRay( transform.position + point * headRadius, Vector3.down*distance, Color.blue);
		}
	}

	private float CalculateSignalStrength(Target t) => (Vector3.Distance(transform.position, t.transform.position) /
	                                                    distance) * t.GetSignalStrength();


	
}