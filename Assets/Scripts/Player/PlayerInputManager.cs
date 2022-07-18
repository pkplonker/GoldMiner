//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using StuartHeathTools;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
///PlayerInput full description
/// </summary>
public class PlayerInputManager : GenericUnitySingleton<PlayerInputManager>
{
	private PlayerControls playerControls;
	public static event Action OnDetectionToggle;
	public static event Action OnManualDetectionToggle;
	private void ManualDetectionToggle(InputAction.CallbackContext obj)=>OnManualDetectionToggle?.Invoke();
	private void Detection(InputAction.CallbackContext obj)=>OnDetectionToggle?.Invoke();
	private void OnDisable()=>playerControls.Disable();
	public Vector2 GetPlayerMovement() => playerControls.PlayerMovement.Move.ReadValue<Vector2>();
	public Vector2 GetMouseDelta() => playerControls.PlayerMovement.Look.ReadValue<Vector2>();
	public bool GetLeftClick() => playerControls.PlayerMovement.LeftClick.inProgress;
	public bool GetRightClick() => playerControls.PlayerMovement.RightClick.inProgress;
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
	}



	


}