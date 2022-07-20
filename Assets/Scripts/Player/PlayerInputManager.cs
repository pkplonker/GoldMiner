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
		private PlayerControls playerControls;
		private bool rightClick;
		private bool leftClick;
		public static event Action OnDetectionToggle;
		public static event Action OnManualDetectionToggle;
		public static event Action OnDiggingToggle;

		private void ManualDetectionToggle(InputAction.CallbackContext obj) => OnManualDetectionToggle?.Invoke();
		private void Detection(InputAction.CallbackContext obj) => OnDetectionToggle?.Invoke();
		private void Digging(InputAction.CallbackContext obj) => OnDiggingToggle?.Invoke();

		private void OnDisable() => playerControls.Disable();
		public Vector2 GetPlayerMovement() => playerControls.PlayerMovement.Move.ReadValue<Vector2>();
		public Vector2 GetMouseDelta() => playerControls.PlayerMovement.Look.ReadValue<Vector2>();
		public Vector2 GetMousePosition() => playerControls.PlayerMovement.MousePosition.ReadValue<Vector2>();

		public bool GetLeftClick() => leftClick;
		public bool GetRightClick() => rightClick;
		public bool GetLeftClickHeld() => playerControls.PlayerMovement.LeftClick.inProgress;
		public bool GetRightClickHeld() => playerControls.PlayerMovement.RightClick.inProgress;

		protected override void Awake()
		{
			base.Awake();
			playerControls = new PlayerControls();
		}

		private void OnEnable()
		{
			playerControls.Enable();
			playerControls.PlayerMovement.DetectionToggle.performed += Detection;
			playerControls.PlayerMovement.ManualDetectionToggle.performed += ManualDetectionToggle;
			playerControls.PlayerMovement.Digging.performed += Digging;
			playerControls.PlayerMovement.LeftClick.performed += _ => leftClick = true;
			playerControls.PlayerMovement.LeftClick.performed += _ => rightClick = true;
		}

		private void LateUpdate()
		{
			leftClick = false;
			rightClick = false;
		}
	}
}