//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using Player;
using UnityEngine;

namespace DetectorScripts
{
	/// <summary>
	///DetectorTest full description
	/// </summary>
	public class DetectorMovement : MonoBehaviour
	{
		[SerializeField] private float maxYRot = 35f;
		[SerializeField] private float rotationSpeed = 50f;
		private float startX = 0f;
		private bool currentTargetIsLeft;
		private Quaternion startRotation;
		private Quaternion finishRotation;

		private void OnEnable()
		{
			DetectorState.RegisterDetector(this);
			PlayerInteractionStateMachine.OnDetectorManualToggleChanged += OnDetectorAutoToggle;
		}

		private void OnDetectorAutoToggle(bool on)
		{
			if (!on) return;
			var localRotation = transform.localRotation;
			currentTargetIsLeft = !(Quaternion.Angle(localRotation, startRotation) >
			                        Quaternion.Angle(localRotation, finishRotation));
		}

		public void HandleAutomaticMovement()
		{
			if (currentTargetIsLeft)
			{
				HandleLeftMove();
				if (Math.Abs(transform.localEulerAngles.y - startRotation.eulerAngles.y) < 0.1f)
					currentTargetIsLeft = false;
			}
			else
			{
				HandleRightMove();
				if (Math.Abs(transform.localEulerAngles.y - finishRotation.eulerAngles.y) < 0.1f)
					currentTargetIsLeft = true;
			}
		}


		private void OnDisable()
		{
			DetectorState.UnregisterDetector(this);
			PlayerInteractionStateMachine.OnDetectorManualToggleChanged -= OnDetectorAutoToggle;
		}

		private void Start()
		{
			var rotation = transform.localRotation;
			startRotation = Quaternion.Euler(rotation.x, rotation.y + (360 - maxYRot), rotation.z);
			finishRotation = Quaternion.Euler(rotation.x, rotation.y + maxYRot, rotation.z);
		}

		private void Update()
		{
			if (!PlayerInteractionStateMachine.isDetecting) ResetPosition();
			else
			{
				if (PlayerInteractionStateMachine.isManualDetecting) HandleManualMovement();
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
			transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime * -1, Space.Self);
			var x = maxYRot * -1;
			if (!(transform.localEulerAngles.y < 360 - maxYRot) || !(transform.localEulerAngles.y > maxYRot)) return;
			var eulerAngles = transform.localEulerAngles;
			eulerAngles = new Vector3(eulerAngles.x, maxYRot * -1, eulerAngles.z);
			transform.localEulerAngles = eulerAngles;
		}

		private void HandleRightMove()
		{
			transform.Rotate(Vector3.down, rotationSpeed * Time.deltaTime * -1, Space.Self);
			if (!(transform.localEulerAngles.y > maxYRot) || !(transform.localEulerAngles.y < 360 - maxYRot)) return;
			var eulerAngles = transform.localEulerAngles;
			eulerAngles = new Vector3(eulerAngles.x, maxYRot, eulerAngles.z);
			transform.localEulerAngles = eulerAngles;
		}
	}
}