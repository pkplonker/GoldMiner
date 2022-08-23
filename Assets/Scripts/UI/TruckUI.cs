//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using Player;
using StuartHeathTools;

namespace UI
{
	/// <summary>
	///TruckUI full description
	/// </summary>
	public class TruckUI : CanvasGroupBase
	{
		private PlayerInteractionStateMachine _playerInteractionStateMachine;
		private void Awake() => Hide();

		public void Show(PlayerInteractionStateMachine pism)
		{
			_playerInteractionStateMachine = pism;
			ShowUI();
		}

		public void Hide()
		{
			if (_playerInteractionStateMachine != null)
			{
				var pm = _playerInteractionStateMachine.GetComponent<PlayerMovement>();
				if (pm != null) pm.SetCanMove(true);
			}

			HideUI();
		}
	}
}