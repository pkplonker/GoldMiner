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
		public static event Action OnIdleToggle;
		public static event Action OnESC;
		public static event Action OnInvent;

		private void ManualDetectionToggle(InputAction.CallbackContext obj) => OnManualDetectionToggle?.Invoke();
		private void Detection(InputAction.CallbackContext obj) => OnDetectionToggle?.Invoke();
		private void Digging(InputAction.CallbackContext obj) => OnDiggingToggle?.Invoke();
		private void Idle(InputAction.CallbackContext obj) => OnIdleToggle?.Invoke();
		private void ESC(InputAction.CallbackContext obj) => OnESC?.Invoke();
		private void Invent(InputAction.CallbackContext obj) => OnInvent?.Invoke();

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
			_playerControls.PlayerMovement.Idle.performed += Idle;
			_playerControls.PlayerMovement.LeftClick.performed += SetLeftClick;
			_playerControls.PlayerMovement.RightClick.performed += SetRightClick;
			_playerControls.PlayerMovement.ESC.performed += ESC;
			_playerControls.PlayerMovement.Invent.performed += Invent;
		}


		private void OnDisable()
		{
			_playerControls.Disable();
			_playerControls.PlayerMovement.DetectionToggle.performed -= Detection;
			_playerControls.PlayerMovement.ManualDetectionToggle.performed -= ManualDetectionToggle;
			_playerControls.PlayerMovement.Digging.performed -= Digging;
			_playerControls.PlayerMovement.Idle.performed -= Idle;
			_playerControls.PlayerMovement.LeftClick.performed -= SetLeftClick;
			_playerControls.PlayerMovement.RightClick.performed -= SetRightClick;
			_playerControls.PlayerMovement.ESC.performed -= ESC;
			_playerControls.PlayerMovement.Invent.performed -= Invent;
		}

		private void SetLeftClick(InputAction.CallbackContext obj) => _leftClick = true;

		private void SetRightClick(InputAction.CallbackContext obj) => _rightClick = true;


		private void LateUpdate()
		{
			_leftClick = false;
			_rightClick = false;
		}
	}
}