using System;
using UnityEngine;

namespace Player
{
	[RequireComponent(typeof(CharacterController))]
	public class PlayerMovement : MonoBehaviour
	{
		[Header("Player")] [SerializeField] private float _moveSpeed = 4.0f;
		[SerializeField] private float _rotationSpeedX = 12.0f;
		[SerializeField] private float _rotationSpeedY = 12.0f;

		[Tooltip("What layers the character uses as ground")] [SerializeField]
		private LayerMask _groundLayers;

		[SerializeField] private float _speedChangeRate = 10.0f;
		[SerializeField] private float _gravity = -15.0f;
		[SerializeField] private float _fallTimeout = 0.15f;
		[SerializeField] private float _groundedOffset = -0.14f;
		[SerializeField] private float _groundedRadius = 0.5f;
		public event Action<Vector2> OnMove;
		public event Action<Vector2> OnRotate;

		[SerializeField] private GameObject _cinemachineCameraTarget;
		[SerializeField] private float _topClamp = 89.0f;
		[SerializeField] private float _bottomClamp = -89.0f;

		private float _cinemachineTargetPitch;
		private bool _grounded = true;

		private float _speed;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private const float TERMINAL_VELOCITY = 53.0f;

		private float _fallTimeoutDelta;

		private CharacterController _controller;
		private GameObject _mainCamera;
		private bool _canMove;

		private const float THRESHOLD = 0.01f;


		private void Awake()
		{
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}

			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}

		private void Start()
		{
			_controller = GetComponent<CharacterController>();
			_fallTimeoutDelta = _fallTimeout;
		}

		private void Update()
		{
			Gravity();
			GroundedCheck();
			if (CanMove()) Move();
		}

		private void LateUpdate()
		{
			if (CanMove()) CameraRotation();
		}

		private bool CanMove() => true;
		public void SetCanMove(bool m) => _canMove = m;

		private void GroundedCheck()
		{
			var position = transform.position;
			Vector3 spherePosition = new Vector3(position.x, position.y - _groundedOffset,
				position.z);
			_grounded = Physics.CheckSphere(spherePosition, _groundedRadius, _groundLayers,
				QueryTriggerInteraction.Ignore);
		}

		private void CameraRotation()
		{
			var mouseLook = PlayerInputManager.Instance.GetMouseDelta();
			if (!(mouseLook.sqrMagnitude >= THRESHOLD)) return;
			_cinemachineTargetPitch -= mouseLook.y * _rotationSpeedY * Time.deltaTime;
			_rotationVelocity = mouseLook.x * _rotationSpeedX * Time.deltaTime;
			_cinemachineTargetPitch = ClampCameraPitchAngle(_cinemachineTargetPitch, _bottomClamp, _topClamp);
			_cinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
			transform.Rotate(Vector3.up * _rotationVelocity);
			OnRotate?.Invoke(mouseLook.normalized);
		}

		private void Move()
		{
			var input = PlayerInputManager.Instance.GetPlayerMovement();

			var velocity = _controller.velocity;
			var currentHorizontalSpeed = new Vector3(velocity.x, 0.0f, velocity.z).magnitude;
			const float SPEED_OFFSET = 0.1f;
			if (currentHorizontalSpeed < _moveSpeed - SPEED_OFFSET || currentHorizontalSpeed > _moveSpeed + SPEED_OFFSET)
			{
				_speed = Mathf.Lerp(currentHorizontalSpeed, _moveSpeed, Time.deltaTime * _speedChangeRate);
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else _speed = _moveSpeed;


			var inputDirection = new Vector3(input.x, 0.0f, input.y).normalized;
			if (input != Vector2.zero)
			{
				var trans = transform;
				inputDirection = trans.right * input.x + trans.forward * input.y;
			}

			_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) +
			                 new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
			OnMove?.Invoke(input.normalized);
		}

		private void Gravity()
		{
			if (_grounded)
			{
				_fallTimeoutDelta = _fallTimeout;
				if (_verticalVelocity < 0.0f) _verticalVelocity = -2f;
			}
			else
			{
				if (_fallTimeoutDelta >= 0.0f) _fallTimeoutDelta -= Time.deltaTime;
			}

			if (_verticalVelocity < TERMINAL_VELOCITY) _verticalVelocity += _gravity * Time.deltaTime;
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
			Gizmos.color = _grounded ? transparentGreen : transparentRed;
			var position = transform.position;
			Gizmos.DrawSphere(
				new Vector3(position.x, position.y - _groundedOffset, position.z),
				_groundedRadius);
		}
	}
}