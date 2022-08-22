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
		[SerializeField] private float _maxYRot = 35f;
		[SerializeField] private float _rotationSpeed = 50f;
		private float _startX = 0f;
		private bool _currentTargetIsLeft;
		private Quaternion _startRotation;
		private Quaternion _finishRotation;

		private void OnEnable()
		{
			DetectorState.RegisterDetector(this);
			PlayerInteractionStateMachine.OnDetectorManualToggleChanged += OnDetectorAutoToggle;
		}

		private void OnDetectorAutoToggle(bool on)
		{
			if (!on) return;
			var localRotation = transform.localRotation;
			_currentTargetIsLeft = !(Quaternion.Angle(localRotation, _startRotation) >
			                        Quaternion.Angle(localRotation, _finishRotation));
		}

		public void HandleAutomaticMovement()
		{
			if (_currentTargetIsLeft)
			{
				MoveLeft();
				if (Math.Abs(transform.localEulerAngles.y - _startRotation.eulerAngles.y) < 0.1f)
					_currentTargetIsLeft = false;
			}
			else
			{
				MoveRight();
				if (Math.Abs(transform.localEulerAngles.y - _finishRotation.eulerAngles.y) < 0.1f)
					_currentTargetIsLeft = true;
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
			_startRotation = Quaternion.Euler(rotation.x, rotation.y + (360 - _maxYRot), rotation.z);
			_finishRotation = Quaternion.Euler(rotation.x, rotation.y + _maxYRot, rotation.z);
		}

		private void Update()
		{
			if (!PlayerInteractionStateMachine.IsDetecting) ResetPosition();
			else
			{
				if (PlayerInteractionStateMachine.IsManualDetecting) HandleManualMovement();
				else HandleAutomaticMovement();
			}
		}

		public void ResetPosition()
		{
			var eulerAngles = transform.localEulerAngles;
			eulerAngles = new Vector3(_startX, eulerAngles.y, eulerAngles.z);
			transform.localEulerAngles = eulerAngles;
		}


		public void HandleManualMovement()
		{
			if (PlayerInputManager.Instance.GetLeftClickHeld() && PlayerInputManager.Instance.GetRightClickHeld()) return;
			if (PlayerInputManager.Instance.GetLeftClickHeld()) MoveLeft();
			else if (PlayerInputManager.Instance.GetRightClickHeld()) MoveRight();
		}

		private void MoveLeft()
		{
			transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime * -1, Space.Self);
			var x = _maxYRot * -1;
			if (!(transform.localEulerAngles.y < 360 - _maxYRot) || !(transform.localEulerAngles.y > _maxYRot)) return;
			var eulerAngles = transform.localEulerAngles;
			eulerAngles = new Vector3(eulerAngles.x, _maxYRot * -1, eulerAngles.z);
			transform.localEulerAngles = eulerAngles;
		}

		private void MoveRight()
		{
			transform.Rotate(Vector3.down, _rotationSpeed * Time.deltaTime * -1, Space.Self);
			if (!(transform.localEulerAngles.y > _maxYRot) || !(transform.localEulerAngles.y < 360 - _maxYRot)) return;
			var eulerAngles = transform.localEulerAngles;
			eulerAngles = new Vector3(eulerAngles.x, _maxYRot, eulerAngles.z);
			transform.localEulerAngles = eulerAngles;
		}
	}
}