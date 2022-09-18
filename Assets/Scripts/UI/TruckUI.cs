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

		public void Init(PlayerInteractionStateMachine pism)
		{
			pism.GetComponent<PlayerMovement>().SetCanMove(false);
			_playerInteractionStateMachine = pism;
		}

		public override void Hide()
		{
			base.Hide();
			_playerInteractionStateMachine.GetComponent<PlayerMovement>().SetCanMove(true);
		}


	}
}