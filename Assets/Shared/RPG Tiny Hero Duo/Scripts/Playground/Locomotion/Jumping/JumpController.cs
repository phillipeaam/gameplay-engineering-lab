using System;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Locomotion;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion.Grounding;
using UnityEngine;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion.Jumping
{
    internal sealed class JumpController
    {
        private const float GroundedVerticalVelocity = -2f;

        private readonly IJumpSettings _settings;
        private readonly IJumpAnimator _jumpAnimator;

        private bool _hasDoubleJumpAvailable;
        private bool _jumpRequested;

        public float VerticalVelocity { get; private set; }

        public JumpController(
            IJumpSettings jumpSettings,
            IJumpAnimator jumpAnimator)
        {
            _settings = jumpSettings.RequireValid();

            _jumpAnimator = jumpAnimator
                ?? throw new ArgumentNullException(nameof(jumpAnimator));

            _hasDoubleJumpAvailable = jumpSettings.CanDoubleJump;
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
            if (stateChange.HasChanged || _hasDoubleJumpAvailable != _settings.CanDoubleJump)
            {
                _hasDoubleJumpAvailable = _settings.CanDoubleJump;
            }
        }

        public void ProcessRequest(bool isGrounded, bool isJumpBlocked)
        {
            if (!_jumpRequested)
            {
                return;
            }

            _jumpRequested = false;

            if (isJumpBlocked)
            {
                return;
            }

            if (isGrounded)
            {
                ApplyJumpVelocity();
                _jumpAnimator.RequestJump();
                return;
            }

            if (_settings.CanDoubleJump && _hasDoubleJumpAvailable)
            {
                _hasDoubleJumpAvailable = false;
                ApplyJumpVelocity();
                _jumpAnimator.RequestJump();
            }
        }

        private void ApplyJumpVelocity()
        {
            VerticalVelocity = Mathf.Sqrt(_settings.JumpHeight * -2f * Physics.gravity.y);
        }

        public void CancelJumpRequest()
        {
            _jumpRequested = false;
        }
    }
}
