//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace DetectorScripts
{
	/// <summary>
	///DetectorTest full description
	/// </summary>
	public class DetectorMovement : MonoBehaviour
	{
		[SerializeField] private float maxYRot = 35f;
		[SerializeField] private float rotationSpeed = 35f;
		private float startX = 0f;
		private bool currentTargetIsLeft;
		private Quaternion startRotation;
		private Quaternion finishRotation;
		private DetectorCollisionAvoidance detectorCollisionAvoidance;

		private void Awake() => detectorCollisionAvoidance = GetComponentInChildren<DetectorCollisionAvoidance>();

		private void OnEnable()
		{
			DetectorState.RegisterDetector(this);
			PlayerInteractionStateMachine.OnDetectorManualToggleChanged += OnDetectorAutoToggle;
		}

		private void OnDisable()
		{
			DetectorState.UnregisterDetector(this);
			PlayerInteractionStateMachine.OnDetectorManualToggleChanged -= OnDetectorAutoToggle;
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
				MoveLeft();
				if (Math.Abs(transform.localEulerAngles.y - startRotation.eulerAngles.y) < 0.1f)
					currentTargetIsLeft = false;
			}
			else
			{
				MoveRight();
				if (Math.Abs(transform.localEulerAngles.y - finishRotation.eulerAngles.y) < 0.1f)
					currentTargetIsLeft = true;
			}
		}

		private void Start()
		{
			var rotation = transform.localRotation;
			startRotation = Quaternion.Euler(rotation.x, rotation.y + (360 - maxYRot), rotation.z);
			finishRotation = Quaternion.Euler(rotation.x, rotation.y + maxYRot, rotation.z);
		}

		private void Update()
		{
			if (!PlayerInteractionStateMachine.IsDetecting)
			{
				ResetPosition();
			}
			else
			{
				if (PlayerInteractionStateMachine.IsManualDetecting) HandleManualMovement();
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
			var inputManager = ServiceLocator.Instance.GetService<PlayerInputManager>();
			if (inputManager.GetLeftClickHeld() || inputManager.GetPanLeftHeld()) MoveLeft();
			else if (inputManager.GetRightClickHeld() || inputManager.GetPanRightHeld()) MoveRight();
		}

		private void MoveLeft()
		{
			var originalRotation = transform.rotation;
			ApplyLeftMovement();
			ValidateMovement(originalRotation);
		}

		private void ValidateMovement(Quaternion originalRotation)
		{
			if (!detectorCollisionAvoidance.CanMove())
			{
				transform.rotation = originalRotation;
				currentTargetIsLeft = !currentTargetIsLeft;
			}
		}

		private void MoveRight()
		{
			var originalRotation = transform.rotation;
			ApplyRightMovement();
			ValidateMovement(originalRotation);
		}

		private void ApplyLeftMovement()
		{
			ApplyRotation(Vector3.up);
			if (!(transform.localEulerAngles.y < 360 - maxYRot) || !(transform.localEulerAngles.y > maxYRot)) return;
			ApplyAngleChange(maxYRot * -1);
		}

		private void ApplyRightMovement()
		{
			ApplyRotation(Vector3.down);
			if (!(transform.localEulerAngles.y > maxYRot) || !(transform.localEulerAngles.y < 360 - maxYRot)) return;
			ApplyAngleChange(maxYRot);
		}

		private void ApplyRotation(Vector3 dir) =>
			transform.Rotate(dir, rotationSpeed * Time.deltaTime * -1, Space.Self);

		private void ApplyAngleChange(float rot)
		{
			var eulerAngles = transform.localEulerAngles;
			eulerAngles = new Vector3(eulerAngles.x, rot, eulerAngles.z);
			transform.localEulerAngles = eulerAngles;
		}
	}
}