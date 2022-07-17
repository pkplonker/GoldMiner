//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

/// <summary>
///DetectorTest full description
/// </summary>
public class Detector : MonoBehaviour
{
	[Range(0.01f, 0.8f)] [SerializeField] private float maxYRot = 30f;
	[SerializeField] private float autoMoveSpeed = 2f;
	[SerializeField] private float manualMoveSpeed = 50f;
	[SerializeField] private Quaternion autoMoveStartRot;
	private float startX = 0f;
	private bool currentTargetIsLeft;

	private Quaternion startRotation;
	private Quaternion finishRotation;

	private void OnEnable()
	{
		DetectorState.RegisterDetector(this);
		DetectorState.OnDetectorManualToggleChanged += OnDetectorAutoToggle;
	}

	private void OnDetectorAutoToggle(bool obj)
	{
		autoMoveStartRot = transform.localRotation;

		var s = Quaternion.Angle(autoMoveStartRot, startRotation);
		var f = Quaternion.Angle(autoMoveStartRot, finishRotation);
		Debug.Log(s + " " + f);
		currentTargetIsLeft = !(s > f);
	}

	public void HandleAutomaticMovement()
	{
		if (currentTargetIsLeft)
		{
			HandleLeftMove();
			if (Math.Abs(transform.localEulerAngles.y - startRotation.eulerAngles.y) < 0.1f)
			{
				currentTargetIsLeft = false;
				Debug.Log("Switched to right");
			}
		}
		else
		{
			HandleRightMove();
			if (Math.Abs(transform.localEulerAngles.y - finishRotation.eulerAngles.y) < 0.1f)
			{
				currentTargetIsLeft = true;
				Debug.Log("Switched to left");
			}
		}
	}


	private void OnDisable()
	{
		DetectorState.UnregisterDetector(this);
		DetectorState.OnDetectorManualToggleChanged -= OnDetectorAutoToggle;
	}

	private void Start()
	{
		var rotation = transform.localRotation;
		startRotation = Quaternion.Euler(rotation.x, rotation.y + (360 - maxYRot), rotation.z);
		finishRotation = Quaternion.Euler(rotation.x, rotation.y + maxYRot, rotation.z);
	}

	private void Update()
	{
		if (!DetectorState.isDetecting) ResetPosition();
		else
		{
			if (DetectorState.isManualDetecting) HandleManualMovement();
			else HandleAutomaticMovement();
		}
	}

	public void ResetPosition()
	{
		var eulerAngles = transform.localEulerAngles;
		eulerAngles = new Vector3(startX, eulerAngles.y, eulerAngles.z);
		transform.localEulerAngles = eulerAngles;
	}


	public void HandleManualMovement()
	{
		if (PlayerInputManager.Instance.GetLeftClick() && PlayerInputManager.Instance.GetRightClick()) return;
		if (PlayerInputManager.Instance.GetLeftClick()) HandleLeftMove();
		else if (PlayerInputManager.Instance.GetRightClick()) HandleRightMove();
	}

	private void HandleLeftMove()
	{
		transform.Rotate(Vector3.up, manualMoveSpeed * Time.deltaTime * -1, Space.Self);
		var x = maxYRot * -1;
		if (transform.localEulerAngles.y < 360 - maxYRot && transform.localEulerAngles.y > maxYRot)
		{
			var eulerAngles = transform.localEulerAngles;
			eulerAngles = new Vector3(eulerAngles.x, maxYRot * -1, eulerAngles.z);
			transform.localEulerAngles = eulerAngles;
		}
	}

	private void HandleRightMove()
	{
		transform.Rotate(Vector3.down, manualMoveSpeed * Time.deltaTime * -1, Space.Self);
		if (transform.localEulerAngles.y > maxYRot && transform.localEulerAngles.y < 360 - maxYRot)
		{
			var eulerAngles = transform.localEulerAngles;
			eulerAngles = new Vector3(eulerAngles.x, maxYRot, eulerAngles.z);
			transform.localEulerAngles = eulerAngles;
		}
	}
}