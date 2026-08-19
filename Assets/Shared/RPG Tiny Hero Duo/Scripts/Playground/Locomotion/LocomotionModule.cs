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
        private readonly IMovementAnimator _movementAnimator;
        private readonly LocomotionInputEvents _inputEvents;
        private readonly CharacterControllerMotor _movementMotor;
        private readonly LandingRecovery _landingRecovery;
        private readonly GroundedStateTracker _groundedStateTracker = new();
        private readonly JumpController _jumpController;

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

            _movementAnimator = movementAnimator
                ?? throw new ArgumentNullException(nameof(movementAnimator));

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

            _inputEvents = new LocomotionInputEvents(locomotionInput);
            _movementMotor = new CharacterControllerMotor(characterController, movementSettings);
            _landingRecovery = new LandingRecovery(landingSettings);
            _jumpController = new JumpController(jumpSettings, jumpAnimator);
        }

        public void Enable()
        {
            if (_isEnabled)
            {
                return;
            }

            _inputEvents.MovementChanged += OnMovementChanged;
            _inputEvents.JumpRequested += OnJumpRequested;
            _inputEvents.LookChanged += OnLookChanged;
            _inputEvents.Enable();

            _isEnabled = true;
        }

        public void Disable()
        {
            if (!_isEnabled)
            {
                return;
            }

            _inputEvents.MovementChanged -= OnMovementChanged;
            _inputEvents.JumpRequested -= OnJumpRequested;
            _inputEvents.LookChanged -= OnLookChanged;
            _inputEvents.Disable();

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
            _jumpController.RequestJump();
        }

        private void OnLookChanged(Vector2 look)
        {
            _lookInput = look;
        }

        public void Tick(float deltaTime)
        {
            _landingRecovery.Tick(deltaTime);

            _jumpController.UpdateVerticalVelocity(_movementMotor.IsGrounded, deltaTime);

            var movementMultiplier = _landingRecovery.GetMovementMultiplier();

            var collisionFlags = _movementMotor.Move(
                _movementInput,
                _jumpController.VerticalVelocity,
                movementMultiplier,
                deltaTime);

            var stateChange = UpdateGroundedState(collisionFlags);

            _jumpController.ProcessRequest(
                stateChange.IsGrounded,
                isJumpBlocked: _landingRecovery.IsActive);

            _movementMotor.Rotate(_lookInput, deltaTime);
        }

        private GroundedStateChange UpdateGroundedState(CollisionFlags collisionFlags)
        {
            var stateChange = _groundedStateTracker.Update(collisionFlags);

            if (stateChange.HasLanded)
            {
                _landingRecovery.Start();
            }

            _jumpController.UpdateGroundedState(stateChange);

            if (stateChange.HasChanged)
            {
                _movementAnimator.SetGrounded(stateChange.IsGrounded);
            }

            return stateChange;
        }

        private void CancelJumpRequest()
        {
            _jumpController.CancelJumpRequest();
        }
    }
}
