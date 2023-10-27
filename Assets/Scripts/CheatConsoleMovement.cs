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
	private bool cachedState;

	private void Start()
	{
		CheatUIController.CheatConsoleActive += StopMovement;
	}

	private void StopMovement(bool active)
	{
		if (playerReference.GetPlayer() != null)
		{
			if (active)
			{
				playerReference.GetPlayer().GetComponent<PlayerMovement>().SetCanMove(cachedState);
			}
			else
			{
				cachedState = playerReference.GetPlayer().GetComponent<PlayerMovement>().GetCanMove();
				playerReference.GetPlayer().GetComponent<PlayerMovement>().SetCanMove(false);
			}
		}
	}

	private void Update() { }
}