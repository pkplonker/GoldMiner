//
// Copyright (C) 2022 Stuart Heath. All rights reserved.
//

using System;
using Player;
using StuartHeathTools;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace DetectorScripts
{
	/// <summary>
	///DetectionState full description
	/// </summary>
	public class DetectorState : GenericUnitySingleton<DetectorState>
	{
		public static bool isDetecting;
		public static bool isManualDetecting;
		[SerializeField] private Transform rigHandTarget;
		[SerializeField] private Transform handleIKTarget;
		[SerializeField] private Animator animator;
		[SerializeField] private Rig rig;
		private static Detector detector;
		public static event Action<bool> OnDetectorToggleChanged;
		public static event Action<bool> OnDetectorManualToggleChanged;


		private void OnEnable()
		{
			PlayerInputManager.OnDetectionToggle += ToggleDetection;
			PlayerInputManager.OnManualDetectionToggle += ManualDetectionToggle;
		
		}

		private void OnDisable()
		{
			PlayerInputManager.OnDetectionToggle -= ToggleDetection;
			PlayerInputManager.OnManualDetectionToggle -= ManualDetectionToggle;
		}

		private void Start()
		{
			OnDetectorManualToggleChanged?.Invoke(isManualDetecting);
			OnDetectorToggleChanged?.Invoke(isDetecting);
		}

		private void Update()
		{
			UpdateHandIK();
			if (detector == null) return;
			if (!isDetecting) detector.ResetPosition();
			else
			{
				if (isManualDetecting) detector.HandleManualMovement();
				else detector.HandleAutomaticMovement();
			}
		}

		public static void RegisterDetector(Detector d) => detector = d;


		public static void UnregisterDetector(Detector d)
		{
			if (d == detector) detector = null;
			else Debug.Log("Trying to deregister a detector that is not registered");
		}

		private void ManualDetectionToggle()
		{
			isManualDetecting = !isManualDetecting;
			OnDetectorManualToggleChanged?.Invoke(isManualDetecting);
	
		}

		private void ToggleDetection()
		{
			isDetecting = !isDetecting;
			OnDetectorToggleChanged?.Invoke(isDetecting);
		} 

		private void UpdateHandIK()
		{
			rigHandTarget.position = handleIKTarget.position;
			if (!isDetecting)
			{
				animator.SetLayerWeight(animator.GetLayerIndex("RightHand"), 0);
				rig.weight = 0;
				return;
			}

			animator.SetLayerWeight(animator.GetLayerIndex("RightHand"), 1);
			rig.weight = 1;
		}
	}
}