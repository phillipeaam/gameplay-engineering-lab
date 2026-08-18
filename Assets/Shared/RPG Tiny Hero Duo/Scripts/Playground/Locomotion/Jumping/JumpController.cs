using Shared.RPG_Tiny_Hero_Duo.Scripts.Locomotion;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion.Grounding;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion.Landing;
using UnityEngine;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion.Jumping
{
    internal sealed class JumpController
    {
        private const float GroundedVerticalVelocity = -2f;

        private readonly ILocomotionConfiguration _configuration;
        private readonly ILocomotionAnimator _animator;
        private readonly LandingRecovery _landingRecovery;

        private bool _hasDoubleJumpAvailable;
        private bool _jumpRequested;

        public float VerticalVelocity { get; private set; }

        public JumpController(
            ILocomotionConfiguration configuration,
            ILocomotionAnimator animator,
            LandingRecovery landingRecovery)
        {
            _configuration = configuration;
            _animator = animator;
            _landingRecovery = landingRecovery;
            _hasDoubleJumpAvailable = configuration.CanDoubleJump;
        }

        public void RequestJump()
        {
            _jumpRequested = true;
        }

        public void UpdateVerticalVelocity(bool isGrounded, float deltaTime)
        {
            // CharacterController does not apply gravity automatically. Before calculating
            // the next movement, use the grounded state from the last Move() call.
            if (isGrounded && VerticalVelocity < GroundedVerticalVelocity)
            {
                // A small downward velocity keeps the controller firmly touching the ground
                // and helps it behave consistently on slopes and uneven surfaces.
                VerticalVelocity = GroundedVerticalVelocity;
            }

            // Apply gravity for this frame. Multiplying by deltaTime keeps the result
            // consistent across different frame rates.
            VerticalVelocity += Physics.gravity.y * deltaTime;
        }

        public void UpdateGroundedState(GroundedStateChange stateChange)
        {
            if (!stateChange.IsGrounded)
            {
                return;
            }

            // Touching the ground starts a new jump sequence, so the double jump becomes
            // available again. The setting is read here so runtime changes are respected.
            if (stateChange.HasChanged || _hasDoubleJumpAvailable != _configuration.CanDoubleJump)
            {
                _hasDoubleJumpAvailable = _configuration.CanDoubleJump;
            }
        }

        public void ProcessRequest(bool isGrounded)
        {
            if (!_jumpRequested)
            {
                return;
            }

            _jumpRequested = false;

            if (_landingRecovery.TimeRemaining > 0f)
            {
                return;
            }

            if (isGrounded)
            {
                ApplyJumpVelocity();
                _animator.RequestJump();
                return;
            }

            if (_configuration.CanDoubleJump && _hasDoubleJumpAvailable)
            {
                _hasDoubleJumpAvailable = false;
                ApplyJumpVelocity();
                _animator.RequestJump();
            }
        }

        private void ApplyJumpVelocity()
        {
            // The formula comes from the constant-acceleration motion equation:
            // finalSpeed² = initialSpeed² + 2 × acceleration × distance.
            // At the jump apex, finalSpeed is zero, so:
            // initialSpeed = squareRoot(jumpHeight × -2 × gravity).
            VerticalVelocity = Mathf.Sqrt(_configuration.JumpHeight * -2f * Physics.gravity.y);
        }
        
        public void CancelJumpRequest()
        {
            _jumpRequested = false;
        }
    }
}
