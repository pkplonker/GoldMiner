//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using Player;
using StuartHeathTools;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
	/// <summary>
	///TruckUI full description
	/// </summary>
	public class TruckUI : TweenUIPanel
	{
		private PlayerInteractionStateMachine _playerInteractionStateMachine;
		[SerializeField] private Button _firstSelectedGameObject;
		public void Init(PlayerInteractionStateMachine pism)
		{
			pism.GetComponent<PlayerMovement>().SetCanMove(false);
			_playerInteractionStateMachine = pism;
		}

		public override void Show()
		{
			base.Show();
			EventSystem.current.SetSelectedGameObject(_firstSelectedGameObject.gameObject);
		}

		public override void Hide()
		{
			base.Hide();
			_playerInteractionStateMachine.GetComponent<PlayerMovement>().SetCanMove(true);
		}


	}
}