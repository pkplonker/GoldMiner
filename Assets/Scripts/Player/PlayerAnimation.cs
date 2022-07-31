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
		 private PlayerMovement playerMovement;
		 private void Awake()
		 {
			 playerMovement=GetComponentInParent<PlayerMovement>();
			 GetAnimator();
		 }

		 private void GetAnimator()
		 {
			 animator = GetComponent<Animator>();
		 }

		 private void OnEnable()
		 {
			 playerMovement.OnMove += Move;
			 playerMovement.OnRotate += Rotate;

		 }

		 private void OnDisable()
		 {
			 playerMovement.OnMove += Move;
			 playerMovement.OnRotate += Rotate;
		 }

		 private void Rotate(Vector2 v)
		 {
		   

		 }

		 private void Move(Vector2 v)
		 {
			 if (!animator)
			 {
				 GetAnimator();
			 }
			 animator.SetFloat("MoveX",v.x);
			 animator.SetFloat("MoveY",v.y);
		 }
	 }
 }
