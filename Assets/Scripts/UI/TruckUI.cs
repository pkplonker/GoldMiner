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
		private PlayerInteractionStateMachine playerInteractionStateMachine;
		[SerializeField] private Button firstSelectedGameObject;
		public void Init(PlayerInteractionStateMachine pism)
		{
			pism.GetComponent<PlayerMovement>().SetCanMove(false);
			playerInteractionStateMachine = pism;
		}

		public override void Show()
		{
			base.Show();
			EventSystem.current.SetSelectedGameObject(firstSelectedGameObject.gameObject);
		}

		public override void Hide()
		{
			base.Hide();
			playerInteractionStateMachine.GetComponent<PlayerMovement>().SetCanMove(true);
		}


	}
}