using NUnit.Framework;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Locomotion;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion.Input;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion.Landing;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Tests.PlayMode
{
    public sealed class LocomotionModuleLifecycleTests : InputTestFixture
    {
        private GameObject _character;
        private TestLocomotionInput _input;
        private LocomotionModule _locomotion;

        [TearDown]
        public override void TearDown()
        {
            _locomotion?.Disable();
            _input?.Dispose();

            if (_character != null)
            {
                Object.DestroyImmediate(_character);
            }

            base.TearDown();
        }

        /// <summary>
        /// Verifies that disabling locomotion cancels a pending jump request so the
        /// character does not jump by itself when locomotion is enabled again.
        /// </summary>
        [Test]
        public void Disable_WithPendingJump_DoesNotJumpAfterReenable()
        {
            var gamepad = InputSystem.AddDevice<Gamepad>();
            CreateLocomotion(out var animator);

            _locomotion.Enable();
            Press(gamepad.buttonSouth);

            _locomotion.Disable();
            Release(gamepad.buttonSouth);
            _locomotion.Enable();
            _locomotion.Tick(0f);

            Assert.That(animator.JumpRequestCount, Is.Zero);
        }

        /// <summary>
        /// Verifies that disabling locomotion clears movement and look input so the
        /// character does not move, rotate, or retain movement animation after reactivation.
        /// </summary>
        [Test]
        public void Disable_WithMovementAndLookInput_ClearsInputBeforeReenable()
        {
            var gamepad = InputSystem.AddDevice<Gamepad>();
            CreateLocomotion(out var animator);

            _locomotion.Enable();
            Set(gamepad.leftStick, Vector2.up);
            Set(gamepad.rightStick, Vector2.right);

            _locomotion.Disable();
            Set(gamepad.leftStick, Vector2.zero);
            Set(gamepad.rightStick, Vector2.zero);
            _locomotion.Enable();

            var positionBeforeTick = _character.transform.position;
            var rotationBeforeTick = _character.transform.rotation;
            _locomotion.Tick(1f);

            Assert.That(animator.LastMovement, Is.EqualTo(Vector2.zero));
            Assert.That(_character.transform.position.x, Is.EqualTo(positionBeforeTick.x).Within(0.0001f));
            Assert.That(_character.transform.position.z, Is.EqualTo(positionBeforeTick.z).Within(0.0001f));
            Assert.That(_character.transform.rotation, Is.EqualTo(rotationBeforeTick));
        }

        private void CreateLocomotion(out RecordingLocomotionAnimator animator)
        {
            _character = new GameObject(nameof(LocomotionModuleLifecycleTests));
            var characterController = _character.AddComponent<CharacterController>();
            animator = new RecordingLocomotionAnimator();
            _input = new TestLocomotionInput();
            var configuration = new TestLocomotionConfiguration();

            _locomotion = new LocomotionModule(
                characterController,
                animator,
                _input,
                configuration,
                configuration);
        }

        private sealed class TestLocomotionInput : ILocomotionInput, System.IDisposable
        {
            public TestLocomotionInput()
            {
                Move = new InputAction(type: InputActionType.Value, binding: "<Gamepad>/leftStick");
                Look = new InputAction(type: InputActionType.Value, binding: "<Gamepad>/rightStick");
                Jump = new InputAction(type: InputActionType.Button, binding: "<Gamepad>/buttonSouth");
            }

            public InputAction Move { get; }
            public InputAction Look { get; }
            public InputAction Jump { get; }

            public void Dispose()
            {
                Move.Dispose();
                Look.Dispose();
                Jump.Dispose();
            }
        }

        private sealed class TestLocomotionConfiguration : ILocomotionConfiguration, ILandingSettings
        {
            public float MovementSpeed => 5f;
            public float RotationSpeed => 250f;
            public float JumpHeight => 1.5f;
            public bool CanDoubleJump => true;
            public float LandingDuration => 0.4f;
            public float LandingMovementMultiplier => 0.2f;
        }

        private sealed class RecordingLocomotionAnimator : ILocomotionAnimator
        {
            public Vector2 LastMovement { get; private set; }
            public int JumpRequestCount { get; private set; }

            public void ApplyMovement(Vector2 movement)
            {
                LastMovement = movement;
            }

            public void SetGrounded(bool isGrounded)
            {
            }

            public void RequestJump()
            {
                JumpRequestCount++;
            }
        }
    }
}
