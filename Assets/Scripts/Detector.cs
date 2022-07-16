//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations.Rigging;

/// <summary>
///DetectorTest full description
/// </summary>
public class Detector : MonoBehaviour
{
	[Range(0.01f, 0.8f)] [SerializeField] private float maxYRot = 30f;
	[SerializeField] private float autoMoveSpeed = 2f;
	[SerializeField] private float manualMoveSpeed = 25f;

	private float startX = 0f;
	[SerializeField] private bool isManualMovement;
	[SerializeField] private bool isDetecting;
	[SerializeField] private Transform rigHandTarget;
	[SerializeField] private Transform handleIKTarget;
	[SerializeField] private Animator animator;
	[SerializeField] private Rig rig;
	[SerializeField] private Quaternion startRotation;
	[SerializeField] private Quaternion finishRotation;

	private void Start()
	{
		var rotation = transform.rotation;
		startRotation = Quaternion.Euler(rotation.x, rotation.y + (360 - maxYRot), rotation.z);
		finishRotation = Quaternion.Euler(rotation.x, rotation.y + maxYRot, rotation.z);
	}

	private void Update()
	{
		UpdateHandIK();
		if (!isDetecting) ResetPosition();
		else
		{
			if (isManualMovement) HandleManualMovement();
			else HandleAutomaticMovement();
		}
	}

	private void ResetPosition()
	{
		var eulerAngles = transform.eulerAngles;
		eulerAngles = new Vector3(startX, eulerAngles.y, eulerAngles.z);
		transform.eulerAngles = eulerAngles;
	}

	private void HandleAutomaticMovement()
	{
		//rotate between startRotation and finishRotation with movespeed

		var lerp = 0.5F * (1.0F + Mathf.Sin(Mathf.PI * Time.realtimeSinceStartup * autoMoveSpeed));
		transform.localRotation = Quaternion.Lerp(startRotation, finishRotation, lerp);
	}

	private void HandleManualMovement()
	{
		if (PlayerInputManager.Instance.GetLeftClick() && PlayerInputManager.Instance.GetRightClick()) return;


		if (PlayerInputManager.Instance.GetLeftClick())
		{
			HandleLeftMove();
		}

		if (PlayerInputManager.Instance.GetRightClick())
		{
			HandleRightMove();
		}
	}

	private void HandleLeftMove()
	{
		transform.Rotate(Vector3.up, manualMoveSpeed * Time.deltaTime * -1);
		Debug.Log(Clamp0360(transform.eulerAngles.y));
		var x = maxYRot * -1;
		if (transform.eulerAngles.y < 360-maxYRot && transform.eulerAngles.y>maxYRot)
		{
			Debug.Log("Hit left limit");
			var eulerAngles = transform.eulerAngles;
			eulerAngles = new Vector3(eulerAngles.x, maxYRot * -1, eulerAngles.z);
			transform.eulerAngles = eulerAngles;
		}
	}

	private void HandleRightMove()
	{
		transform.Rotate(Vector3.down, manualMoveSpeed * Time.deltaTime*-1);
		Debug.Log(Clamp0360(transform.eulerAngles.y));
		if (transform.eulerAngles.y > maxYRot && transform.eulerAngles.y<360-maxYRot)
		{
			Debug.Log("Hit right limit");
			var eulerAngles = transform.eulerAngles;
			eulerAngles = new Vector3(eulerAngles.x, maxYRot, eulerAngles.z);
			transform.eulerAngles = eulerAngles;
		}
		
	}

	private void UpdateHandIK()
	{
		rigHandTarget.position = handleIKTarget.position;
		if (!isDetecting)
		{
			animator.SetLayerWeight(animator.GetLayerIndex("RightHand"), 0);
			rig.weight = 0;
			return;
		}

		animator.SetLayerWeight(animator.GetLayerIndex("RightHand"), 1);
		rig.weight = 1;
	}
	
	public static float Clamp0360(float eulerAngles)
	{
		float result = eulerAngles - Mathf.CeilToInt(eulerAngles / 360f) * 360f;
		if (result < 0)
		{
			result += 360f;
		}
		return result;
	}
}