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
	private void Update()
	{
		Physics.SphereCast(transform.position, headRadius, Vector3.down, out var raycastHit, distance);
		if (raycastHit.collider == null) return;
		if (raycastHit.collider.GetComponent<Target>() == null) return;
		Debug.Log("Buzzzzz");
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawSphere(transform.position, headRadius);
		Gizmos.DrawSphere(transform.position-(Vector3.down*distance), headRadius);

	}
    }
