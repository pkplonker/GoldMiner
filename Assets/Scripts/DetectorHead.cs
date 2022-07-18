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
	public static event Action<float> OnDetection;
	private void GenerateCone()
	{
		GameObject go = new GameObject("Cone");
		go.transform.SetParent(transform);
		go.transform.localPosition = Vector3.zero;
		go.transform.localRotation =  Quaternion.Euler(-90,0,0);
		go.transform.localScale = transform.localScale;
		var cone = go.AddComponent<Cone>();
		cone.GenerateCone(transform.localPosition, 12, distance,lowerRad,upperRad);
		
		//debug only
		
		go.AddComponent<MeshRenderer>();


	}
	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.F))
		{
			GenerateCone();
		}
		if (!DetectorState.isDetecting) return;
		
		
		//OnDetection?.Invoke(x);
	}
	

	private float CalculateSignalStrength(Target t) => (Vector3.Distance(transform.position, t.transform.position) /
	                                                    distance) * t.GetSignalStrength();


	
}