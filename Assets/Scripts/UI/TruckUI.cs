//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using Player;
using StuartHeathTools;

namespace UI
{
	/// <summary>
	///TruckUI full description
	/// </summary>
	public class TruckUI : TweenUIPanel
	{
		private PlayerInteractionStateMachine _playerInteractionStateMachine;

		public void Show(PlayerInteractionStateMachine pism)
		{
			_playerInteractionStateMachine = pism;
			ShowUI();
		}

		public void Hide()=>HideUI(CloseCallback);
		
		private void CloseCallback()
		{
			var pm = _playerInteractionStateMachine.GetComponent<PlayerMovement>();
			if (pm != null) pm.SetCanMove(true);
		}

		
	}
}