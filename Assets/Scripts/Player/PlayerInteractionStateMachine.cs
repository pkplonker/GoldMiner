using System;
using DetectorScripts;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Player
{
	public class PlayerInteractionStateMachine : StateMachine
	{
		public static event Action<BaseState> OnStateChanged;

		[HideInInspector] public Camera Camera;
		public readonly BaseState DiggingState = new Digging();
		public readonly BaseState DetectingState = new DetectorState();
		public readonly BaseState IdleState = new IdleState();
		[Header("Digging")] public SpriteRenderer _diggingTarget;
		public readonly string GROUND_LAYER = "Ground";
		public float _maxDigRange = 2f;
		public float _digDepth = 1f;
		[Header("Detecting")] public Transform _rigHandTarget;
		public Transform _handleIKTarget;
		public Animator _animator;
		public Rig _rig;
		public GameObject _detectorModel;
		public static bool IsDetecting;
		public static bool IsManualDetecting;
		public static event Action<bool> OnDetectorManualToggleChanged;
		[field:Range(0,-20f), SerializeField] public float MaxDigDepth { get; private set; }

		private void Start()
		{
			_animator.SetLayerWeight(_animator.GetLayerIndex("RightHand"), 0);
			_rig.weight = 0;
			IsDetecting = false;
			IsManualDetecting = false;
			_detectorModel.SetActive(false);

		}

		private void OnEnable()
		{
			PlayerInputManager.OnDetectionToggle += ToggleDetection;
			PlayerInputManager.OnManualDetectionToggle += ManualDetectionToggle;
			PlayerInputManager.OnDiggingToggle += DiggingToggle;
		}

		private void ManualDetectionToggle()
		{
			IsManualDetecting = !IsManualDetecting;
			OnDetectorManualToggleChanged?.Invoke(IsManualDetecting);
		}

		private void ToggleDetection()
		{
			IsDetecting = !IsDetecting;
			ChangeState(IsDetecting ? DetectingState : IdleState);
		}

		private void OnDisable()
		{
			PlayerInputManager.OnDetectionToggle -= ToggleDetection;
			PlayerInputManager.OnManualDetectionToggle -= ManualDetectionToggle;
			PlayerInputManager.OnDiggingToggle -= DiggingToggle;
		}

		private void DiggingToggle()
		{
			ChangeState(DiggingState);
		}

		private void Awake()
		{
			Camera = Camera.main;
			ChangeState(IdleState);

		}

		protected override void ChangeState(BaseState state)
		{
			base.ChangeState(state);
			OnStateChanged?.Invoke(CurrentState);
		}
	}
}