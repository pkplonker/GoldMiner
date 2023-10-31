using System;
using Player;
using TerrainGeneration;
using UnityEngine;

namespace UI
{
	public class HUDUI : MonoBehaviour, IShowHideUI
	{
		private CanvasGroup canvasGroup;

		private void Awake()
		{
			canvasGroup = GetComponent<CanvasGroup>();
			Hide();
		}

		private void OnEnable() => ServiceLocator.Instance.GetService<MapGenerator>().MapGenerated += StartGame;

		private void OnDisable() => ServiceLocator.Instance.GetService<MapGenerator>().MapGenerated -= StartGame;

		private void StartGame(float f) => Toggle();

		public void Toggle()
		{
			canvasGroup.alpha = 1;
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
		}

		public void Hide()
		{
			canvasGroup.alpha = 0;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
		}

		public void Disable()
		{
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
		}

		public void Enable()
		{
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
		}
	}
}