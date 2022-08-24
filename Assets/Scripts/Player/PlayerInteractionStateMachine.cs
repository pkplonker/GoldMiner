using System;
using DetectorScripts;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

namespace Player
{
	public class PlayerInteractionStateMachine : StateMachine
	{
		public static event Action<BaseState> OnStateChanged;

		[HideInInspector] public Camera Camera;

		public readonly BaseState DiggingState = new Digging();
		public readonly BaseState DetectingState = new DetectorState();
		public readonly BaseState InteractState = new InteractState();
		[Header("Digging")] public SpriteRenderer _diggingTarget;
		public readonly string GROUND_LAYER = "Ground";
		[field: SerializeField] public float digRange { get; private set; } = 2f;
		[field: SerializeField] public float interactionRange { get; private set; } = 2f;

		[field: SerializeField] public float DigDepth { get; private set; } = 1f;

		[field: Header("Detecting"), SerializeField]
		public Transform RigHandTarget { get; private set; }

		[field: SerializeField] public Transform HandleIKTarget { get; private set; }
		[field: SerializeField] public Animator Animator { get; private set; }
		[field: SerializeField] public Rig Rig { get; private set; }
		[field: SerializeField] public GameObject DetectorModel { get; private set; }
		[field: SerializeField] public Image Reticle { get; private set; }
		[SerializeField] private PlayerReference _playerReference;
		public static bool IsDetecting;
		public static bool IsManualDetecting;
		public event Action OnPlayerDestroyed;
		public static event Action<bool> OnDetectorManualToggleChanged;

		[field: Range(0, -20f), SerializeField]
		public float MaxDigDepth { get; private set; }

		private void Start()
		{
			Animator.SetLayerWeight(Animator.GetLayerIndex("RightHand"), 0);
			Rig.weight = 0;
			IsDetecting = false;
			IsManualDetecting = false;
			DetectorModel.SetActive(false);
		}

		private void OnEnable()
		{
			_playerReference.SetPlayer(this); 
			PlayerInputManager.OnDetectionToggle += ToggleDetection;
			PlayerInputManager.OnManualDetectionToggle += ManualDetectionToggle;
			PlayerInputManager.OnDiggingToggle += DiggingToggle;
			PlayerInputManager.OnIdleToggle += InteractionToggle;
		}

		private void InteractionToggle() => ChangeState(InteractState);

		private void OnDestroy()
		{
			_playerReference.SetPlayer(null);
			OnPlayerDestroyed?.Invoke();
		}


		private void ManualDetectionToggle()
		{
			if (CurrentState != DetectingState) return;
			IsManualDetecting = !IsManualDetecting;
			OnDetectorManualToggleChanged?.Invoke(IsManualDetecting);
		}

		private void ToggleDetection()
		{
			IsDetecting = !IsDetecting;
			ChangeState(IsDetecting ? DetectingState : InteractState);
		}

		private void OnDisable()
		{
			PlayerInputManager.OnDetectionToggle -= ToggleDetection;
			PlayerInputManager.OnManualDetectionToggle -= ManualDetectionToggle;
			PlayerInputManager.OnDiggingToggle -= DiggingToggle;
			PlayerInputManager.OnIdleToggle -= InteractionToggle;
		}

		private void DiggingToggle() => ChangeState(DiggingState);


		private void Awake()
		{
			Camera = Camera.main;
			ChangeState(InteractState);
		}

		protected override void ChangeState(BaseState state)
		{
			base.ChangeState(state);
			OnStateChanged?.Invoke(CurrentState);
		}
	}
}