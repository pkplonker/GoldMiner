//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using StuartHeathTools;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	/// <summary>
	///PlayerInput full description
	/// </summary>
	public class PlayerInputManager : GenericUnitySingleton<PlayerInputManager>
	{
		private PlayerControls _playerControls;
		private bool _rightClick;
		private bool _leftClick;
		public static event Action OnDetectionToggle;
		public static event Action OnManualDetectionToggle;
		public static event Action OnDiggingToggle;

		private void ManualDetectionToggle(InputAction.CallbackContext obj) => OnManualDetectionToggle?.Invoke();
		private void Detection(InputAction.CallbackContext obj) => OnDetectionToggle?.Invoke();
		private void Digging(InputAction.CallbackContext obj) => OnDiggingToggle?.Invoke();

		private void OnDisable() => _playerControls.Disable();
		public Vector2 GetPlayerMovement() => _playerControls.PlayerMovement.Move.ReadValue<Vector2>();
		public Vector2 GetMouseDelta() => _playerControls.PlayerMovement.Look.ReadValue<Vector2>();
		public Vector2 GetMousePosition() => _playerControls.PlayerMovement.MousePosition.ReadValue<Vector2>();

		public bool GetLeftClick() => _leftClick;
		public bool GetRightClick() => _rightClick;
		public bool GetLeftClickHeld() => _playerControls.PlayerMovement.LeftClick.inProgress;
		public bool GetRightClickHeld() => _playerControls.PlayerMovement.RightClick.inProgress;

		protected override void Awake()
		{
			base.Awake();
			_playerControls = new PlayerControls();
		}

		private void OnEnable()
		{
			_playerControls.Enable();
			_playerControls.PlayerMovement.DetectionToggle.performed += Detection;
			_playerControls.PlayerMovement.ManualDetectionToggle.performed += ManualDetectionToggle;
			_playerControls.PlayerMovement.Digging.performed += Digging;
			_playerControls.PlayerMovement.LeftClick.performed += _ => _leftClick = true;
			_playerControls.PlayerMovement.LeftClick.performed += _ => _rightClick = true;
		}

		private void LateUpdate()
		{
			_leftClick = false;
			_rightClick = false;
		}
	}
}