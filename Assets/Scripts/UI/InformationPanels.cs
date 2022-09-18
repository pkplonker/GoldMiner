using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Player;
using UnityEngine;

namespace UI
{
	public class InformationPanels : MonoBehaviour
	{
		private bool _isActive;
		[SerializeField] private float _fadeTime = 0.5f;
		[SerializeField] private List<IFadeUI> fadableUIs;
private play
		private void Awake()
		{
			player.GetComponent<PlayerMovement>()
		} fadableUIs = GetComponentsInChildren<IFadeUI>().ToList();
		private void Start() => HideImmediate();
		private void OnEnable() => PlayerInputManager.OnInvent += ToggleVisibility;
		private void OnDisable() => PlayerInputManager.OnInvent -= ToggleVisibility;

		private void HideImmediate()
		{
			foreach (var ui in fadableUIs)
			{
				ui.FadeOut(0);
			}
		}
		
		private void ToggleVisibility()
		{
			if (_isActive) Hide();
			else Show();
		}

		private void Show()
		{
			_isActive = true;
			foreach (var ui in fadableUIs)
			{
				ui.FadeIn(_fadeTime);
			}
		}

		private void Hide()
		{
			_isActive = false;
			foreach (var ui in fadableUIs)
			{
				ui.FadeOut(_fadeTime);
			}
		}
	}
}