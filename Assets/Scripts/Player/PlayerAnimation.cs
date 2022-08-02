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
		 private Animator _animator;
		 private PlayerMovement _playerMovement;
		 private void Awake()
		 {
			 _playerMovement=GetComponentInParent<PlayerMovement>();
			 GetAnimator();
		 }

		 private void GetAnimator()
		 {
			 _animator = GetComponent<Animator>();
		 }

		 private void OnEnable()
		 {
			 _playerMovement.OnMove += Move;
			 _playerMovement.OnRotate += Rotate;

		 }

		 private void OnDisable()
		 {
			 _playerMovement.OnMove += Move;
			 _playerMovement.OnRotate += Rotate;
		 }

		 private void Rotate(Vector2 v)
		 {
		   

		 }

		 private void Move(Vector2 v)
		 {
			 if (!_animator)
			 {
				 GetAnimator();
			 }
			 _animator.SetFloat("MoveX",v.x);
			 _animator.SetFloat("MoveY",v.y);
		 }
	 }
 }
