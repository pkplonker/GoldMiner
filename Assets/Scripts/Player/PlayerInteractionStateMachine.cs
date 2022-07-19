using System;
using DetectorScripts;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Player
{
	public class PlayerInteractionStateMachine : StateMachine
	{
		public static event Action<BaseState> OnStateChanged;

		[HideInInspector] public Camera camera;
		public BaseState diggingState = new Digging();
		public BaseState detectingState = new DetectorState();
		public BaseState idleState = new IdleState();
		[Header("Digging")] public SpriteRenderer diggingTarget;
		public readonly string GROUND_LAYER = "Ground";
		public float maxDigRange = 2f;

		[Header("Detecting")] public Transform rigHandTarget;
		public Transform handleIKTarget;
		public Animator animator;
		public Rig rig;
		public GameObject detectorModel;
		public static bool isDetecting;
		public static bool isManualDetecting;
		public static event Action<bool> OnDetectorManualToggleChanged;

		private void Start()
		{
			animator.SetLayerWeight(animator.GetLayerIndex("RightHand"), 0);
			rig.weight = 0;
			isDetecting = false;
			isManualDetecting = false;
			detectorModel.SetActive(false);

		}

		private void OnEnable()
		{
			PlayerInputManager.OnDetectionToggle += ToggleDetection;
			PlayerInputManager.OnManualDetectionToggle += ManualDetectionToggle;
			PlayerInputManager.OnDiggingToggle += DiggingToggle;
		}

		private void ManualDetectionToggle()
		{
			isManualDetecting = !isManualDetecting;
			OnDetectorManualToggleChanged?.Invoke(isManualDetecting);
		}

		private void ToggleDetection()
		{
			isDetecting = !isDetecting;
			ChangeState(isDetecting ? detectingState : idleState);
		}

		private void OnDisable()
		{
			PlayerInputManager.OnDetectionToggle -= ToggleDetection;
			PlayerInputManager.OnManualDetectionToggle -= ManualDetectionToggle;
			PlayerInputManager.OnDiggingToggle -= DiggingToggle;
		}

		private void DiggingToggle()
		{
			ChangeState(diggingState);
		}

		private void Awake()
		{
			camera = Camera.main;
			ChangeState(idleState);

		}

		protected override void ChangeState(BaseState state)
		{
			base.ChangeState(state);
			OnStateChanged?.Invoke(currentState);
		}
	}
}