//
// Copyright (C) 2023 Stuart Heath. All rights reserved.
//

using Player;
using UnityEngine;
using PlayerInputManager = Player.PlayerInputManager;

/// <summary>
///DebugController full description
/// </summary>
public class DebugController : MonoBehaviour
{
#if UNITY_EDITOR
	public PlayerReference playerReference;
	private bool showConsole;

	private void Awake()
	{
		PlayerInputManager.OnDebug += OnToggleDebug;
	}

	private void OnDisable()
	{
		PlayerInputManager.OnDebug += OnToggleDebug;
	}

	public void OnToggleDebug()
	{
		showConsole = !showConsole;
		playerReference.GetPlayer().CanMove = !showConsole;
	}

	private void OnGUI()
	{
		if (!showConsole) return;
		float y = 0f;
		GUI.Box(new Rect(0, y, Screen.width, 30), "");
	}
#endif
}