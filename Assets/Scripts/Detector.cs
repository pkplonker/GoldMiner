using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Detector : MonoBehaviour
{
	[SerializeField] private bool detecting = true;

	[SerializeField] private Transform rigHandTarget;
	[SerializeField] private Transform handleIKTarget;
	[SerializeField] private Animator animator;
	[SerializeField] private Rig rig;
	private float directionTimer;
	[SerializeField] private float directionTime = 2f;
	private bool movingLeft;
	[SerializeField] private Transform leftPos;
	[SerializeField] private Transform rightPos;
	[SerializeField] private float rotDegrees = 80f;
	private Vector3 leftRot;
	private Vector3 rightRot;


	private void Start()
	{
		leftRot = transform.localEulerAngles - new Vector3(0, rotDegrees / 2, 0);
		rightRot = transform.localEulerAngles + new Vector3(0, rotDegrees / 2, 0);
	}


	private void Update()
	{
		UpdateHandIK();
		MoveDetector();
	}

	private void MoveDetector()
	{
		//determine if moving left or right
		directionTimer += Time.deltaTime;
		if (directionTimer >= directionTime)
		{
			movingLeft = !movingLeft;
			directionTimer = 0;
		}

		var interpolationRatio = directionTimer / directionTime;
		Debug.Log(interpolationRatio);
		if (movingLeft)
		{
			transform.position = Vector3.Lerp(rightPos.position, leftPos.position, interpolationRatio);
			transform.localEulerAngles = Vector3.Lerp(rightRot, leftRot, interpolationRatio);
		}
		else
		{
			transform.position = Vector3.Lerp(leftPos.position, rightPos.position, interpolationRatio);
			transform.localEulerAngles = Vector3.Lerp(leftRot, rightRot, interpolationRatio);
		}
	}

	private void UpdateHandIK()
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
	}
}