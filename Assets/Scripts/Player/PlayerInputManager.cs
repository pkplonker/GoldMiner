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
	public class PlayerInputManager : MonoBehaviour, IService
	{
		private PlayerControls playerControls;
		private bool leftClick;
		public static event Action OnDetection;
		public static event Action OnManualDetectionToggle;
		public static event Action OnDiggingToggle;
		public static event Action OnJump;

		public static event Action OnIdleToggle;
		public static event Action OnESC;
		public static event Action OnInvent;
		public static event Action OnMap;
#if UNITY_EDITOR
		public static event Action OnDebug;
#endif
		private void ManualDetectionToggle(InputAction.CallbackContext obj) => OnManualDetectionToggle?.Invoke();
		private void Detection(InputAction.CallbackContext obj) => OnDetection?.Invoke();
		private void Jump(InputAction.CallbackContext obj) => OnJump?.Invoke();

		private void Digging(InputAction.CallbackContext obj) => OnDiggingToggle?.Invoke();
		private void Idle(InputAction.CallbackContext obj) => OnIdleToggle?.Invoke();
		private void ESC(InputAction.CallbackContext obj) => OnESC?.Invoke();
		private void Invent(InputAction.CallbackContext obj) => OnInvent?.Invoke();
		private void Map(InputAction.CallbackContext obj) => OnMap?.Invoke();
#if UNITY_EDITOR
		private void DebugMenu(InputAction.CallbackContext obj) => OnDebug?.Invoke();
#endif
		public Vector2 GetPlayerMovement() => playerControls.PlayerMovement.Move.ReadValue<Vector2>();
		public Vector2 GetMouseDelta() => playerControls.PlayerMovement.Look.ReadValue<Vector2>();
		public Vector2 GetMousePosition() => playerControls.PlayerMovement.MousePosition.ReadValue<Vector2>();

		public bool GetLeftClick() => leftClick;
		public bool GetLeftClickHeld() => playerControls.PlayerMovement.LeftClick.inProgress;
		public bool GetPanLeftHeld() => playerControls.PlayerMovement.PanLeft.inProgress;
		public bool GetPanRightHeld() => playerControls.PlayerMovement.PanRight.inProgress;

		protected void Awake()
		{
			playerControls = new PlayerControls();
		}

		private void OnEnable()
		{
			playerControls.Enable();
			playerControls.PlayerMovement.Detection.performed += Detection;
			playerControls.PlayerMovement.Jump.performed += Jump;

			playerControls.PlayerMovement.ManualDetectionToggle.performed += ManualDetectionToggle;
			playerControls.PlayerMovement.Digging.performed += Digging;
			playerControls.PlayerMovement.Idle.performed += Idle;
			playerControls.PlayerMovement.LeftClick.performed += SetLeftClick;
			playerControls.PlayerMovement.ESC.performed += ESC;
			playerControls.PlayerMovement.Invent.performed += Invent;
			playerControls.PlayerMovement.Map.performed += Map;
#if UNITY_EDITOR
			playerControls.PlayerMovement.Debug.performed += DebugMenu;
#endif
		}

		private void OnDisable()
		{
			playerControls.Disable();
			playerControls.PlayerMovement.Detection.performed -= Detection;
			playerControls.PlayerMovement.Jump.performed -= Jump;

			playerControls.PlayerMovement.ManualDetectionToggle.performed -= ManualDetectionToggle;
			playerControls.PlayerMovement.Digging.performed -= Digging;
			playerControls.PlayerMovement.Idle.performed -= Idle;
			playerControls.PlayerMovement.LeftClick.performed -= SetLeftClick;
			playerControls.PlayerMovement.ESC.performed -= ESC;
			playerControls.PlayerMovement.Invent.performed -= Invent;
			playerControls.PlayerMovement.Map.performed -= Map;
#if UNITY_EDITOR
			playerControls.PlayerMovement.Debug.performed -= DebugMenu;
#endif
		}

		private void SetLeftClick(InputAction.CallbackContext obj) => leftClick = true;

		private void LateUpdate()
		{
			leftClick = false;
		}

		public void Initialize()
		{
		}
	}
}