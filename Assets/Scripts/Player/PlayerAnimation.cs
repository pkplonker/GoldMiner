 //
 // Copyright (C) 2022 Stuart Heath. All rights reserved.
 //

 using UnityEngine;

 namespace Player
 {
	 /// <summary>
	 ///PlayerAnimation full description
	 /// </summary>
    
	 public class PlayerAnimation : MonoBehaviour
	 {
		 private Animator animator;
		 private void Awake()
		 {
			 animator = GetComponent<Animator>();
		 }

		 private void OnEnable()
		 {
			 PlayerMovement.OnMove += Move;
			 PlayerMovement.OnRotate += Rotate;

		 }

		 private void OnDisable()
		 {
			 PlayerMovement.OnMove += Move;
			 PlayerMovement.OnRotate += Rotate;
		 }

		 private void Rotate(Vector2 v)
		 {
		   

		 }

		 private void Move(Vector2 v)
		 {
			 animator.SetFloat("MoveX",v.x);
			 animator.SetFloat("MoveY",v.y);
		 }
	 }
 }
