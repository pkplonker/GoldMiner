//
// Copyright (C) 2023 Stuart Heath. All rights reserved.
//

using Player;
using UnityEngine;

/// <summary>
///CheatConsoleMovement full description
/// </summary>
public class CheatConsoleMovement : MonoBehaviour
{
	[SerializeField] private PlayerReference playerReference;

	private void Start()
	{
		CheatUIController.CheatConsoleActive += CheatConsoleActive;
	}

	private void CheatConsoleActive(bool active)
	{
		if (playerReference.GetPlayer() != null)
		{
			playerReference.GetPlayer().GetComponent<PlayerMovement>().SetCanMove(!active);
		}
	}

	private void Update() { }
}