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
	[SerializeField] private float topRadius = 0.4f;
	[SerializeField] private float bottomRadius = 0.4f;

	[SerializeField] private float distance = 0.4f;
	[SerializeField]private Transform headConePoint;
	public static event Action<float> OnDetection;
	private void GenerateCone()
	{
		GameObject go = new GameObject("Cone");
		go.transform.SetParent(headConePoint);
		go.transform.localPosition = headConePoint.localPosition;
		go.transform.localRotation = headConePoint.localRotation;
		go.transform.localScale = headConePoint.localScale;
		var cone = go.AddComponent<Cone>();
		cone.GenerateCone(transform.localPosition,12, distance,topRadius,bottomRadius);
		
		//debug only
		
		go.AddComponent<MeshRenderer>();


	}
	private void Update()
	{
		if (!DetectorState.isDetecting) return;
		
		
		//OnDetection?.Invoke(x);
	}

	private void DrawDebug()
	{
		for (var i = 0; i < 360; i++)
		{
			var angle = i * Mathf.Deg2Rad;
			var x = Mathf.Cos(angle);
			var z = Mathf.Sin(angle);
			var point = new Vector3(x,0, z);
		}
	}

	private float CalculateSignalStrength(Target t) => (Vector3.Distance(transform.position, t.transform.position) /
	                                                    distance) * t.GetSignalStrength();


	
}