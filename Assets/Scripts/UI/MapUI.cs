//
// Copyright (C) 2023 Stuart Heath. All rights reserved.
//

using Player;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///MapUI full description
/// </summary>
namespace UI
{
	[RequireComponent(typeof(CanvasGroup))]
	public class MapUI : TweenUIPanel
	{
		[SerializeField] private RawImage mapImage;
		private bool enabled = false;
		[SerializeField] private HUDUI hud;
		[SerializeField] private PlayerReference playerReference;
		private PlayerMovement playerMovement;
		private InGameMap inGameMap;

		private void OnEnable()
		{
			InGameMap.OnMapGenerated += MapGenerated;
			PlayerInputManager.OnMap += Toggle;
		}

		public override void Toggle()
		{
			if (playerMovement == null) playerMovement = playerReference.GetPlayer().GetComponent<PlayerMovement>();
			enabled = !enabled;
			if (enabled)
			{
				inGameMap.UpdatePlayer();
				ShowUI();
				hud.Hide();
				playerMovement.SetCanMove(false);
			}
			else
			{
				HideUI();
				hud.Toggle();
				playerMovement.SetCanMove(true);
			}
		}

		private void OnDisable()
		{
			InGameMap.OnMapGenerated -= MapGenerated;
			PlayerInputManager.OnMap -= Toggle;
		}

		private void MapGenerated(InGameMap inGameMap, Texture2D texture2D)
		{
			this.inGameMap = inGameMap;
			mapImage.texture = texture2D;
		}
	}
}