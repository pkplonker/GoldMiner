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
		private PlayerMovement _playerMovement;
		public void Init(PlayerInteractionStateMachine pism)
		{
			_playerMovement = pism.GetComponent<PlayerMovement>();
			_playerMovement.SetCanMove(false);
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
			_playerMovement.SetCanMove(true);
		}


	}
}