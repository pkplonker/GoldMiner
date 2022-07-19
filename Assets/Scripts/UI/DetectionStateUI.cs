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
		 [SerializeField] private TextMeshProUGUI text;

		private void Awake()
		{
		}

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
			text.text = "PlayerState = "+state.GetType();
		}
	}
}