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
	[SerializeField] private float targetRotAmount = 45;
	private bool movingLeft = true;
	[SerializeField] private float rotSpeed = 1f;
	private float rotAmount;
	private float leftRotTarget;
	private float rightRotTarget;

	private void Start()
	{
		leftRotTarget = 360 - targetRotAmount;
		rightRotTarget = targetRotAmount;
	}

	private void Update()
	{
		UpdateHandIK();
		MoveDetector();
	}

	private void MoveDetector()
	{
		if (!detecting) return;
		var y = transform.localEulerAngles.y;
		if (movingLeft && y < leftRotTarget && y > rightRotTarget)
		{ 
				movingLeft = false;
			
		}
		else
		{
			if (y > rightRotTarget && y < leftRotTarget)
			{
				//move left
				movingLeft = true;
			}
		}

		if (movingLeft)
		{
			transform.Rotate(Vector3.up, -(rotSpeed * Time.deltaTime), Space.Self);
		}
		else
		{
			transform.Rotate(Vector3.up, (rotSpeed * Time.deltaTime), Space.Self);
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