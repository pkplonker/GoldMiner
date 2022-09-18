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
		private bool _leftClick;
		public static event Action OnDetection;
		public static event Action OnManualDetectionToggle;
		public static event Action OnDiggingToggle;
		public static event Action OnJump;

		public static event Action OnIdleToggle;
		public static event Action OnESC;
		public static event Action OnInvent;

		private void ManualDetectionToggle(InputAction.CallbackContext obj) => OnManualDetectionToggle?.Invoke();
		private void Detection(InputAction.CallbackContext obj) => OnDetection?.Invoke();
		private void Jump(InputAction.CallbackContext obj) => OnJump?.Invoke();

		private void Digging(InputAction.CallbackContext obj) => OnDiggingToggle?.Invoke();
		private void Idle(InputAction.CallbackContext obj) => OnIdleToggle?.Invoke();
		private void ESC(InputAction.CallbackContext obj) => OnESC?.Invoke();
		private void Invent(InputAction.CallbackContext obj) => OnInvent?.Invoke();

		public Vector2 GetPlayerMovement() => _playerControls.PlayerMovement.Move.ReadValue<Vector2>();
		public Vector2 GetMouseDelta() => _playerControls.PlayerMovement.Look.ReadValue<Vector2>();
		public Vector2 GetMousePosition() => _playerControls.PlayerMovement.MousePosition.ReadValue<Vector2>();

		public bool GetLeftClick() => _leftClick;
		public bool GetLeftClickHeld() => _playerControls.PlayerMovement.LeftClick.inProgress;
		public bool GetPanLeftHeld() => _playerControls.PlayerMovement.PanLeft.inProgress;
		public bool GetPanRightHeld() => _playerControls.PlayerMovement.PanRight.inProgress;

		protected override void Awake()
		{
			base.Awake();
			_playerControls = new PlayerControls();
		}

		private void OnEnable()
		{
			_playerControls.Enable();
			_playerControls.PlayerMovement.Detection.performed += Detection;
			_playerControls.PlayerMovement.Jump.performed += Jump;

			_playerControls.PlayerMovement.ManualDetectionToggle.performed += ManualDetectionToggle;
			_playerControls.PlayerMovement.Digging.performed += Digging;
			_playerControls.PlayerMovement.Idle.performed += Idle;
			_playerControls.PlayerMovement.LeftClick.performed += SetLeftClick;
			_playerControls.PlayerMovement.ESC.performed += ESC;
			_playerControls.PlayerMovement.Invent.performed += Invent;
		}


		private void OnDisable()
		{
			_playerControls.Disable();
			_playerControls.PlayerMovement.Detection.performed -= Detection;
			_playerControls.PlayerMovement.Jump.performed -= Jump;

			_playerControls.PlayerMovement.ManualDetectionToggle.performed -= ManualDetectionToggle;
			_playerControls.PlayerMovement.Digging.performed -= Digging;
			_playerControls.PlayerMovement.Idle.performed -= Idle;
			_playerControls.PlayerMovement.LeftClick.performed -= SetLeftClick;
			_playerControls.PlayerMovement.ESC.performed -= ESC;
			_playerControls.PlayerMovement.Invent.performed -= Invent;
		}

		private void SetLeftClick(InputAction.CallbackContext obj) => _leftClick = true;



		private void LateUpdate()
		{
			_leftClick = false;
		}
	}
}