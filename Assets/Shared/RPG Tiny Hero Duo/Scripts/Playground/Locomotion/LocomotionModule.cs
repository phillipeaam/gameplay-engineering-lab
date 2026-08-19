using System;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Locomotion;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion.Input;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion.Jumping;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion.Landing;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion.Movement;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion.Grounding;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Shared;
using UnityEngine;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion
{
    public sealed class LocomotionModule : IAnimationModule
    {
        private readonly ILandingSettings _landingSettings;
        private readonly IMovementAnimator _movementAnimator;
        private readonly LocomotionInputEvents _input;
        private readonly CharacterControllerMotor _motor;
        private readonly LandingRecovery _landingRecovery = new();
        private readonly GroundedStateTracker _groundedState = new();
        private readonly JumpController _jump;

        private Vector2 _movementInput;
        private Vector2 _lookInput;
        private bool _isEnabled;

        public LocomotionModule(
            CharacterController characterController,
            IMovementAnimator movementAnimator,
            IJumpAnimator jumpAnimator,
            ILocomotionInput locomotionInput,
            IMovementSettings movementSettings,
            IJumpSettings jumpSettings,
            ILandingSettings landingSettings)
        {
            if (characterController == null)
            {
                throw new ArgumentNullException(nameof(characterController));
            }

            _movementAnimator = movementAnimator ?? throw new ArgumentNullException(nameof(movementAnimator));

            if (jumpAnimator == null)
            {
                throw new ArgumentNullException(nameof(jumpAnimator));
            }

            if (locomotionInput == null)
            {
                throw new ArgumentNullException(nameof(locomotionInput));
            }

            if (movementSettings == null)
            {
                throw new ArgumentNullException(nameof(movementSettings));
            }

            if (jumpSettings == null)
            {
                throw new ArgumentNullException(nameof(jumpSettings));
            }
            _landingSettings = landingSettings ?? throw new ArgumentNullException(nameof(landingSettings));

            _input = new LocomotionInputEvents(locomotionInput);
            _motor = new CharacterControllerMotor(characterController, movementSettings);
            _jump = new JumpController(jumpSettings, jumpAnimator, _landingRecovery);
        }

        public void Enable()
        {
            if (_isEnabled)
            {
                return;
            }

            _input.MovementChanged += OnMovementChanged;
            _input.JumpRequested += OnJumpRequested;
            _input.LookChanged += OnLookChanged;
            _input.Enable();

            _isEnabled = true;
        }

        public void Disable()
        {
            if (!_isEnabled)
            {
                return;
            }

            _input.MovementChanged -= OnMovementChanged;
            _input.JumpRequested -= OnJumpRequested;
            _input.LookChanged -= OnLookChanged;
            _input.Disable();

            _isEnabled = false;

            OnMovementChanged(Vector2.zero);
            CancelJumpRequest();
            OnLookChanged(Vector2.zero);
        }

        private void OnMovementChanged(Vector2 movement)
        {
            _movementInput = movement;
            _movementAnimator.ApplyMovement(movement);
        }

        private void OnJumpRequested()
        {
            _jump.RequestJump();
        }

        private void OnLookChanged(Vector2 look)
        {
            _lookInput = look;
        }

        public void Tick(float deltaTime)
        {
            _landingRecovery.Tick(deltaTime);

            _jump.UpdateVerticalVelocity(_motor.IsGrounded, deltaTime);

            var movementMultiplier = _landingRecovery.GetMovementMultiplier(_landingSettings);

            var collisionFlags = _motor.Move(
                _movementInput,
                _jump.VerticalVelocity,
                movementMultiplier,
                deltaTime);

            var stateChange = UpdateGroundedState(collisionFlags);

            _jump.ProcessRequest(stateChange.IsGrounded);

            _motor.Rotate(_lookInput, deltaTime);
        }

        private GroundedStateChange UpdateGroundedState(CollisionFlags collisionFlags)
        {
            var stateChange = _groundedState.Update(collisionFlags);

            if (stateChange.HasLanded)
            {
                _landingRecovery.Start(_landingSettings.LandingDuration);
            }

            _jump.UpdateGroundedState(stateChange);

            if (stateChange.HasChanged)
            {
                _movementAnimator.SetGrounded(stateChange.IsGrounded);
            }

            return stateChange;
        }

        private void CancelJumpRequest()
        {
            _jump.CancelJumpRequest();
        }
    }
}
