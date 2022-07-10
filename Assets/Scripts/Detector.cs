using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Detector : MonoBehaviour
{
	[SerializeField] private bool detecting = true;
	[SerializeField] private float moveRate = 3f;
	[SerializeField] private float rotationRate = 2f;
	private Vector3 leftPosition;
	private Vector3 rightPosition;
	private Vector3 leftRot;
	private Vector3 rightRot;
	[SerializeField] private float rotationAmount;
	[SerializeField] private float moveAmount;
	private float startTime;
	private float totalDistance;
	private bool movingLeft;
	[SerializeField] private Transform rigHandTarget;
	[SerializeField] private Transform handleIKTarget;
	[SerializeField] private Animator animator;
	[SerializeField] private Rig rig;
	void Start()
	{
		var position = transform.localPosition;
		leftPosition = position -
		               new Vector3((moveAmount / 2),0,0);
		rightPosition = position + new Vector3((moveAmount / 2), 0, 0);
		var eulerAngles = transform.eulerAngles;
		leftRot = eulerAngles - new Vector3(0, (rotationAmount / 2), 0);
		rightRot = eulerAngles + new Vector3(0, (rotationAmount / 2), 0);
	}

	// Update is called once per frame    
	/*void Update()
	{
		if (!detecting) return;
		transform.position =
			Vector3.Lerp(transform.position, movingLeft ? leftPosition : rightPosition, Time.deltaTime) * moveRate;
		if (Vector3.Distance(transform.position, leftPosition) < 0.2f) movingLeft = false;
		else if (Vector3.Distance(transform.position, rightPosition) < 0.1f) movingLeft = true;
	}*/
	
	void Update()
	{
		rigHandTarget.position = handleIKTarget.position;
		if (!detecting)
		{
			animator.SetLayerWeight(animator.GetLayerIndex("RightHand"), 0);
			rig.weight = 0;
			return;
		}
		animator.SetLayerWeight(animator.GetLayerIndex("RightHand"), 1);
		rig.weight = 1;

		//PingPong between 0 and 1
		float time = Mathf.PingPong(Time.time * moveRate, 1);
		transform.localPosition = Vector3.Slerp(leftPosition, rightPosition, time);
		transform.localEulerAngles = Vector3.Slerp(leftRot, rightRot, time);

	}
}