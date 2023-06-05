 //
 // Copyright (C) 2022 Stuart Heath. All rights reserved.
 //

 using System;
 using UnityEngine;

 namespace Player
 {
	 /// <summary>
	 ///PlayerReference full description
	 /// </summary>
	 [CreateAssetMenu(fileName = "Player Reference",menuName = "References/PlayerReference")]

	 public class PlayerReference : ScriptableObject
	 {
		 private PlayerInteractionStateMachine Player;
		 public static event Action OnPlayerChanged;
		 public  PlayerInteractionStateMachine GetPlayer() => Player;

		 public void SetPlayer(PlayerInteractionStateMachine player)
		 {
			 Player = player;
			 OnPlayerChanged?.Invoke();
		 }
	 }
 }
