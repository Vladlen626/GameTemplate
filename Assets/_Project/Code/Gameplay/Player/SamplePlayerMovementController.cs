using PlatformCore.Core;
using PlatformCore.Core.Lifecycle;
using PlatformCore.Services.Input;
using PlatformCore.Services.UI;
using UnityEngine;

namespace Project.Gameplay.Player
{
	public sealed class SamplePlayerMovementController : IBaseController, IActivatable, IDeactivatable, IUpdatable
	{
		private readonly SamplePlayerView _playerView;
		private readonly CharacterController _characterController;
		private readonly IInputService _inputService;
		private readonly ICursorService _cursorService;

		private Vector3 _velocity;
		private float _yaw;
		private float _pitch;
		private Vector3 _cameraRootBaseLocalPosition;
		private float _headBobPhase;
		private float _headBobOffsetY;

		public SamplePlayerMovementController(
			SamplePlayerView playerView,
			IInputService inputService,
			ICursorService cursorService)
		{
			_playerView = playerView;
			_inputService = inputService;
			_cursorService = cursorService;
			_characterController = playerView.CharacterController;
		}

		public void Activate()
		{
			if (!_playerView || !_characterController)
			{
				return;
			}

			if (_playerView.CameraRoot)
			{
				_cameraRootBaseLocalPosition = _playerView.CameraRoot.localPosition;
			}

			_yaw = _playerView.transform.eulerAngles.y;
			if (_playerView.Head)
			{
				_pitch = Mathf.DeltaAngle(0f, _playerView.Head.localEulerAngles.x);
			}
		}

		public void Deactivate()
		{
			_velocity = Vector3.zero;
			ResetHeadBob();
		}

		public void OnUpdate(float deltaTime)
		{
			if (!_playerView || !_characterController)
			{
				return;
			}

			if (!_playerView.gameObject.activeInHierarchy)
			{
				return;
			}

			var hasControl = _cursorService.IsCursorLocked;
			var moveInput = hasControl ? _inputService.Move : Vector2.zero;
			var lookInput = hasControl ? _inputService.Look : Vector2.zero;

			UpdateLook(lookInput);
			UpdateMovement(moveInput, hasControl, deltaTime);
			UpdateHeadBob(moveInput, hasControl, deltaTime);
		}

		private void UpdateLook(Vector2 lookInput)
		{
			_yaw += lookInput.x * _playerView.LookSensitivity;
			_playerView.transform.rotation = Quaternion.Euler(0f, _yaw, 0f);

			if (!_playerView.Head)
			{
				return;
			}

			_pitch -= lookInput.y * _playerView.LookSensitivity;
			_pitch = Mathf.Clamp(_pitch, _playerView.MinPitch, _playerView.MaxPitch);
			_playerView.Head.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
		}

		private void UpdateMovement(Vector2 moveInput, bool hasControl, float deltaTime)
		{
			var move =
				_playerView.transform.right * moveInput.x +
				_playerView.transform.forward * moveInput.y;

			_characterController.Move(move * (_playerView.MoveSpeed * deltaTime));

			if (_characterController.isGrounded && _velocity.y < 0f)
			{
				_velocity.y = -2f;
			}

			if (hasControl && _playerView.EnableJump && _characterController.isGrounded &&
			    _inputService.IsJumpPressedThisFrame)
			{
				_velocity.y = Mathf.Sqrt(_playerView.JumpHeight * -2f * _playerView.Gravity);
			}

			_velocity.y += _playerView.Gravity * deltaTime;
			_characterController.Move(_velocity * deltaTime);
		}

		private void UpdateHeadBob(Vector2 moveInput, bool hasControl, float deltaTime)
		{
			if (!_playerView.EnableHeadBob || !_playerView.CameraRoot)
			{
				return;
			}

			var isMoving = hasControl && _characterController.isGrounded && moveInput.sqrMagnitude > 0.01f;
			var targetOffsetY = 0f;

			if (isMoving)
			{
				_headBobPhase += deltaTime * _playerView.HeadBobFrequency;
				targetOffsetY = Mathf.Sin(_headBobPhase) * _playerView.HeadBobAmplitude;
			}

			_headBobOffsetY = Mathf.MoveTowards(
				_headBobOffsetY,
				targetOffsetY,
				_playerView.HeadBobReturnSpeed * deltaTime);

			var localPosition = _cameraRootBaseLocalPosition;
			localPosition.y += _headBobOffsetY;
			_playerView.CameraRoot.localPosition = localPosition;
		}

		private void ResetHeadBob()
		{
			_headBobPhase = 0f;
			_headBobOffsetY = 0f;

			if (!_playerView || !_playerView.CameraRoot)
			{
				return;
			}

			_playerView.CameraRoot.localPosition = _cameraRootBaseLocalPosition;
		}
	}
}
