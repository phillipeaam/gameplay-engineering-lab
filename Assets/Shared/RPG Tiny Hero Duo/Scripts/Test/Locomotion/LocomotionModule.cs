using System;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Locomotion;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Test.Shared;
using UnityEngine;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Test.Locomotion
{
    public sealed class LocomotionModule : IAnimationModule
    {
        private const float GroundedVerticalVelocity = -2f;

        private readonly CharacterController _characterController;
        private readonly ILocomotionSettings _settings;
        private readonly ILocomotionAnimator _animator;
        private readonly LocomotionInputEvents _input;

        private Vector2 _movementInput;
        private Vector2 _lookInput;
        private float _verticalVelocity;
        private bool _hasDoubleJumpAvailable;
        private bool _isEnabled;
        private bool? _previousGroundedState;
        
        public LocomotionModule(
            CharacterController characterController,
            ILocomotionAnimator animator,
            ILocomotionInput input,
            ILocomotionSettings settings)
        {
            _characterController = characterController ?? throw new ArgumentNullException(nameof(characterController));
            _animator = animator ?? throw new ArgumentNullException(nameof(animator));

            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            _hasDoubleJumpAvailable = settings.CanDoubleJump;
            
            _input = new LocomotionInputEvents(input);
        }

        public void Enable()
        {
            if (_isEnabled) return;
            _input.MovementChanged += OnMovementChanged;
            _input.JumpRequested += OnJumpRequested;
            _input.LookChanged += OnLookChanged;
            _input.Enable();
            _isEnabled = true;
        }

        public void Disable()
        {
            if (!_isEnabled) return;
            _input.MovementChanged -= OnMovementChanged;
            _input.JumpRequested -= OnJumpRequested;
            _input.LookChanged -= OnLookChanged;
            _input.Disable();
            _isEnabled = false;
        }

        private void OnMovementChanged(Vector2 movement)
        {
            _movementInput = movement;
            _animator.ApplyMovement(movement);
        }
        
        private void OnJumpRequested()
        {
            if (_characterController.isGrounded)
            {
                ApplyJumpVelocity();
                _animator.RequestJump();
                return;
            }

            if (_settings.CanDoubleJump && _hasDoubleJumpAvailable)
            {
                _hasDoubleJumpAvailable = false;
                ApplyJumpVelocity();
                _animator.RequestJump();
            }
        }
        
        /// <summary>
        /// Calculates the upward speed needed for the character to reach the configured jump height.
        /// The character starts with this vertical speed, then gravity gradually slows the ascent
        /// until the character reaches the highest point of the jump.
        ///
        /// The calculation comes from the constant-acceleration motion equation:
        /// finalSpeed² = initialSpeed² + 2 × acceleration × distance.
        /// At the jump apex, finalSpeed is zero, so the equation becomes:
        /// initialSpeed = squareRoot(jumpHeight × -2 × gravity).
        /// </summary>
        private void ApplyJumpVelocity()
        {
            // Physics.gravity.y is negative, so multiplying it by -2 produces a positive value
            // that can be safely passed to the square root calculation.
            _verticalVelocity = Mathf.Sqrt(_settings.JumpHeight * -2f * Physics.gravity.y);
        }

        private void OnLookChanged(Vector2 look)
        {
            _lookInput = look;
        }

        public void Tick(float deltaTime)
        {
            UpdateVerticalVelocity(deltaTime);
            CollisionFlags collisionFlags = MoveCharacter(deltaTime);
            UpdateGroundedState(collisionFlags);
            ApplyRotation(deltaTime);
        }

        private void UpdateVerticalVelocity(float deltaTime)
        {
            // CharacterController does not apply gravity automatically. Before calculating
            // the next movement, we use the grounded state from the last Move() call.
            if (_characterController.isGrounded)
            {
                // A grounded character should not keep an increasingly negative falling
                // velocity. A small downward velocity keeps the controller firmly touching
                // the ground and helps it behave consistently on slopes and uneven surfaces.
                if (_verticalVelocity < 0f)
                {
                    _verticalVelocity = GroundedVerticalVelocity;
                }
            }

            // Apply gravity for this frame. Multiplying by deltaTime keeps the result
            // consistent across different frame rates.
            _verticalVelocity += Physics.gravity.y * deltaTime;
        }

        private CollisionFlags MoveCharacter(float deltaTime)
        {
            // Input is expressed relative to the character: x is left/right and z is forward/backward.
            var localInputDirection = new Vector3(_movementInput.x, 0f, _movementInput.y);
            var localMovementVelocity = localInputDirection * _settings.MovementSpeed;

            // Convert horizontal movement into world space so forward follows the character's rotation.
            var worldMovementVelocity = _characterController.transform.TransformDirection(localMovementVelocity);

            // Vertical motion is controlled independently by gravity and jump logic.
            worldMovementVelocity.y = _verticalVelocity;

            return _characterController.Move(worldMovementVelocity * deltaTime);
        }

        private void UpdateGroundedState(CollisionFlags collisionFlags)
        {
            // CollisionFlags can contain multiple collision directions at the same time.
            // The bitwise check isolates the Below flag, which represents contact with the ground.
            bool grounded = (collisionFlags & CollisionFlags.Below) != 0;

            // The first call must initialize the cached state. After that, we only need to
            // notify the animator when the character changes between grounded and airborne.
            bool groundedStateChanged = !_previousGroundedState.HasValue || grounded != _previousGroundedState.Value;

            // When grounded, refresh the double-jump state after landing. The availability
            // check also detects changes to CanDoubleJump made during runtime.
            if (grounded && (groundedStateChanged || _hasDoubleJumpAvailable != _settings.CanDoubleJump))
            {
                _hasDoubleJumpAvailable = _settings.CanDoubleJump;
            }

            // Avoid sending the same grounded value to the animator every frame. This keeps
            // the animation state update event-driven while the physics still runs every frame.
            if (groundedStateChanged)
            {
                _animator.SetGrounded(grounded);
                _previousGroundedState = grounded;
            }
        }

        private void ApplyRotation(float deltaTime)
        {
            var yAngle = _lookInput.x * _settings.RotationSpeed * deltaTime;
            _characterController.transform.Rotate(Vector3.up, yAngle, Space.World);
        }
    }
}
