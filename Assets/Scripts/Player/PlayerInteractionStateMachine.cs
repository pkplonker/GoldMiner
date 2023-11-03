using System;
using DetectorScripts;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

namespace Player
{
	[RequireComponent(typeof(PlayerMovement))]
	public class PlayerInteractionStateMachine : StateMachine
	{
		public static event Action<BaseState> OnStateChanged;

		[HideInInspector] public Camera Camera;
		public readonly BaseState UiState = new UIState();
		public readonly BaseState DiggingState = new DiggingState();
		public readonly BaseState DetectingState = new DetectorState();
		public readonly BaseState InteractState = new InteractState();
		public BaseState PreviousState;
		[Header("Digging")] public SpriteRenderer diggingTarget;
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
		[SerializeField] private PlayerReference playerReference;
		public static bool IsDetecting;
		public static bool IsManualDetecting;
		public bool CanMove;
		private PlayerMovement playerMovement;
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
			ChangeState(InteractState);
		}

		private void OnValidate()
		{
			playerMovement = GetComponent<PlayerMovement>();
		}

		private void OnEnable()
		{
			playerReference.SetPlayer(this);
			PlayerInputManager.OnDetection += Detection;
			PlayerInputManager.OnManualDetectionToggle += ManualDetectionToggle;
			PlayerInputManager.OnDiggingToggle += DiggingToggle;
			PlayerInputManager.OnIdleToggle += InteractionToggle;

			if (playerMovement == null)
			{
				playerMovement = GetComponent<PlayerMovement>();
			}

			playerMovement.OnCanMoveChanged += OnPlayerMovementChanged;
			CanMove = playerMovement.GetCanMove();
		}

		private void InteractionToggle() => ChangeState(InteractState);

		private void OnDestroy()
		{
			playerReference.SetPlayer(null);
			OnPlayerDestroyed?.Invoke();
		}

		private void ManualDetectionToggle()
		{
			if (CurrentState != DetectingState) return;
			IsManualDetecting = !IsManualDetecting;
			OnDetectorManualToggleChanged?.Invoke(IsManualDetecting);
		}

		private void Detection()
		{
			IsDetecting = !IsDetecting;
			ChangeState(IsDetecting ? DetectingState : InteractState);
		}
		
		

		private void OnDisable()
		{
			PlayerInputManager.OnDetection -= Detection;
			PlayerInputManager.OnManualDetectionToggle -= ManualDetectionToggle;
			PlayerInputManager.OnDiggingToggle -= DiggingToggle;
			PlayerInputManager.OnIdleToggle -= InteractionToggle;
			if (playerMovement != null) playerMovement.OnCanMoveChanged -= OnPlayerMovementChanged;
		}

		private void OnPlayerMovementChanged(bool v)
		{
			CanMove = v;
			if (CanMove && CurrentState.GetType() == typeof(UIState)) ChangeState(PreviousState ?? InteractState);
			else ChangeState(UiState);
		}

		private void DiggingToggle() => ChangeState(DiggingState);

		private void Awake()
		{
			Camera = Camera.main;
			ChangeState(InteractState);
		}

		public override void ChangeState(BaseState state)
		{
			PreviousState = CurrentState;
			base.ChangeState(state);
			//Debug.Log($"Setting state {state.GetType().Name}");
			OnStateChanged?.Invoke(CurrentState);
		}
	}
}