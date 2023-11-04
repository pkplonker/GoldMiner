using System;
using TerrainGeneration;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
	[RequireComponent(typeof(CharacterController))]
	public class PlayerMovement : MonoBehaviour
	{
		[SerializeField] private float moveSpeed = 4.0f;

		[SerializeField] private float rotationSpeedX = 12.0f;

		[SerializeField] private float rotationSpeedY = 12.0f;

		[Tooltip("What layers the character uses as ground")] [SerializeField]
		private LayerMask groundLayers;

		[SerializeField] private float speedChangeRate = 10.0f;

		[SerializeField] private float gravity = -15.0f;

		[SerializeField] private float fallTimeout = 0.15f;

		[SerializeField] private float groundedOffset = -0.14f;

		[SerializeField] private float groundedRadius = 0.5f;

		public event Action<Vector2> OnMove;
		public event Action<Vector2> OnRotate;
		public event Action<bool> OnCanMoveChanged;

		[SerializeField] private GameObject cinemachineCameraTarget;

		[SerializeField] private float topClamp = 89.0f;

		[SerializeField] private float bottomClamp = -89.0f;

		private float cinemachineTargetPitch;
		private bool grounded = true;

		private float speed;
		private float rotationVelocity;
		private float verticalVelocity;
		private const float TERMINAL_VELOCITY = 53.0f;

		private float fallTimeoutDelta;

		private CharacterController controller;
		private GameObject mainCamera;
		private bool canMove;
		private DetectorCollisionAvoidance detector;
		private PlayerInputManager inputManager;

		private const float THRESHOLD = 0.01f;

		private void Awake()
		{
			detector = GetComponentInChildren<DetectorCollisionAvoidance>();
			inputManager = ServiceLocator.Instance.GetService<PlayerInputManager>();

			if (mainCamera == null)
			{
				mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}

			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
#if UNITY_EDITOR
			moveSpeed *= 3;
#endif
		}

		public void SetCanMove(bool cm)
		{
			canMove = cm;

			if (cm)
			{
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
			}
			else
			{
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
			}

			OnCanMoveChanged?.Invoke(cm);
		}

		private void Start()
		{
			controller = GetComponent<CharacterController>();
			fallTimeoutDelta = fallTimeout;
			SetCanMove(true);
		}

		private void OnEnable() => ServiceLocator.Instance.GetService<MapGenerator>().MapGenerated +=
			MapGeneratorOnMapGenerated;

		private void OnDisable() => ServiceLocator.Instance.GetService<MapGenerator>().MapGenerated -=
			MapGeneratorOnMapGenerated;

		private void MapGeneratorOnMapGenerated(float obj)
		{
			SetCanMove(true);
		}

		private void Update()
		{
			if (!CanMove()) return;
			Gravity();
			GroundedCheck();
			Move();
		}

		private void LateUpdate()
		{
			if (CanMove()) CameraRotation();
		}

		private bool CanMove() => canMove;

		private void GroundedCheck()
		{
			var position = transform.position;
			var spherePosition = new Vector3(position.x, position.y - groundedOffset,
				position.z);
			grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers,
				QueryTriggerInteraction.Ignore);
		}

		private void CameraRotation()
		{
			var mouseLook = inputManager.GetMouseDelta();
			if (!(mouseLook.sqrMagnitude >= THRESHOLD)) return;

			Quaternion originalCameraRotation = cinemachineCameraTarget.transform.localRotation;
			Quaternion originalCharacterRotation = transform.rotation;

			cinemachineTargetPitch -= mouseLook.y * rotationSpeedY * Time.deltaTime;
			rotationVelocity = mouseLook.x * rotationSpeedX * Time.deltaTime;

			cinemachineTargetPitch = ClampCameraPitchAngle(cinemachineTargetPitch, bottomClamp, topClamp);
			cinemachineCameraTarget.transform.localRotation = Quaternion.Euler(cinemachineTargetPitch, 0.0f, 0.0f);
			transform.Rotate(Vector3.up * rotationVelocity);

			if (!detector.CanMove())
			{
				cinemachineCameraTarget.transform.localRotation = originalCameraRotation;
				transform.rotation = originalCharacterRotation;
			}
			else
			{
				OnRotate?.Invoke(mouseLook.normalized);
			}
		}

		private void Move()
		{
			var input = inputManager.GetPlayerMovement();

			var velocity = controller.velocity;
			var currentHorizontalSpeed = new Vector3(velocity.x, 0.0f, velocity.z).magnitude;

			var targetSpeed = CalculateTargetSpeed(input, currentHorizontalSpeed);
			Vector3 potentialMovement = CalculateInputDirection(input) * (targetSpeed * Time.deltaTime);

			Vector3 originalPosition = transform.position;

			controller.Move(potentialMovement + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);

			if (!detector.CanMove())
			{
				transform.position = originalPosition;
				OnMove?.Invoke(Vector2.zero);
			}
			else
			{
				OnMove?.Invoke(input.normalized);
			}
		}

		private float CalculateTargetSpeed(Vector2 input, float currentHorizontalSpeed)
		{
			const float SPEED_OFFSET = 0.1f;
			if (currentHorizontalSpeed < moveSpeed - SPEED_OFFSET || currentHorizontalSpeed > moveSpeed + SPEED_OFFSET)
			{
				return Mathf.Lerp(currentHorizontalSpeed, moveSpeed, Time.deltaTime * speedChangeRate);
			}
			else
			{
				return moveSpeed;
			}
		}

		private Vector3 CalculateInputDirection(Vector2 input)
		{
			if (input != Vector2.zero)
			{
				var trans = transform;
				return trans.right * input.x + trans.forward * input.y;
			}

			return Vector3.zero;
		}

		private void Gravity()
		{
			if (grounded)
			{
				fallTimeoutDelta = fallTimeout;
				if (verticalVelocity < 0.0f) verticalVelocity = -2f;
			}
			else
			{
				if (fallTimeoutDelta >= 0.0f) fallTimeoutDelta -= Time.deltaTime;
			}

			if (verticalVelocity < TERMINAL_VELOCITY) verticalVelocity += gravity * Time.deltaTime;
		}

		private static float ClampCameraPitchAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void OnDrawGizmosSelected()
		{
			var transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			var transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);
			Gizmos.color = grounded ? transparentGreen : transparentRed;
			var position = transform.position;
			Gizmos.DrawSphere(
				new Vector3(position.x, position.y - groundedOffset, position.z),
				groundedRadius);
		}

		public bool GetCanMove() => canMove;
	}
}