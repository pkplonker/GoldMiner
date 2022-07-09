//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using StuartHeathTools;
using UnityEngine;

/// <summary>
///PlayerInput full description
/// </summary>
public class PlayerInputManager : GenericUnitySingleton<PlayerInputManager>
{
	private PlayerControls playerControls;

	protected override void Awake()
	{
		base.Awake();
		playerControls = new PlayerControls();
	}


	private void OnEnable()
	{
		playerControls.Enable();
	}

	private void OnDisable() => playerControls.Disable();

	public Vector2 GetPlayerMovement()
	{
		Debug.Log("Move input = " + playerControls.PlayerMovement.Move.ReadValue<Vector2>());
		return playerControls.PlayerMovement.Move.ReadValue<Vector2>();
	}

	public Vector2 GetMouseDelta() => playerControls.PlayerMovement.Look.ReadValue<Vector2>();
}