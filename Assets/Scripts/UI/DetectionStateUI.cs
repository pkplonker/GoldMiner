using System;
using System.Diagnostics;
using DetectorScripts;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class DetectionStateUI : MonoBehaviour
	{
		 [SerializeField] private TextMeshProUGUI _text;

	
		private void OnEnable()
		{
			PlayerInteractionStateMachine.OnStateChanged += StateChange;
		}

		private void OnDisable()
		{
			PlayerInteractionStateMachine.OnStateChanged -= StateChange;
		}

		private void StateChange(BaseState state)
		{
			_text.text = "PlayerState = "+state.GetType();
		}
	}
}