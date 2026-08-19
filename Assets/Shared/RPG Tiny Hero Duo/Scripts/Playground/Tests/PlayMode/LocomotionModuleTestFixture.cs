using NUnit.Framework;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Locomotion;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion.Input;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion.Jumping;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion.Landing;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion.Movement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Tests.PlayMode
{
    public abstract class LocomotionModuleTestFixture : InputTestFixture
    {
        protected GameObject Character;
        protected TestLocomotionInput Input;
        protected LocomotionModule Locomotion;

        [TearDown]
        public override void TearDown()
        {
            Locomotion?.Disable();
            Input?.Dispose();

            if (Character != null)
            {
                Object.DestroyImmediate(Character);
            }

            base.TearDown();
        }

        protected void CreateLocomotion(out RecordingLocomotionAnimator animator)
        {
            CreateLocomotion(new TestLocomotionConfiguration(), out animator);
        }

        protected void CreateLocomotion(TestLocomotionConfiguration configuration)
        {
            CreateLocomotion(configuration, out _);
        }

        protected void CreateLocomotion(
            TestLocomotionConfiguration configuration,
            out RecordingLocomotionAnimator animator)
        {
            Character = new GameObject(GetType().Name);
            var characterController = Character.AddComponent<CharacterController>();
            animator = new RecordingLocomotionAnimator();
            Input = new TestLocomotionInput();

            Locomotion = new LocomotionModule(
                characterController,
                animator,
                animator,
                Input,
                configuration,
                configuration,
                configuration);
        }

        protected sealed class TestLocomotionInput : ILocomotionInput, System.IDisposable
        {
            public TestLocomotionInput(bool includeJumpAction = true)
            {
                Move = new InputAction(type: InputActionType.Value, binding: "<Gamepad>/leftStick");
                Look = new InputAction(type: InputActionType.Value, binding: "<Gamepad>/rightStick");
                Jump = includeJumpAction
                    ? new InputAction(type: InputActionType.Button, binding: "<Gamepad>/buttonSouth")
                    : null;
            }

            public InputAction Move { get; }
            public InputAction Look { get; }
            public InputAction Jump { get; }

            public void Dispose()
            {
                Move.Dispose();
                Look.Dispose();
                Jump?.Dispose();
            }
        }

        protected sealed class TestLocomotionConfiguration :
            IMovementSettings,
            IJumpSettings,
            ILandingSettings
        {
            public float MovementSpeed { get; set; } = 5f;
            public float RotationSpeed { get; set; } = 250f;
            public float JumpHeight { get; set; } = 1.5f;
            public bool CanDoubleJump { get; set; } = true;
            public float LandingDuration { get; set; } = 0.4f;
            public float LandingMovementMultiplier { get; set; } = 0.2f;
        }

        protected sealed class RecordingLocomotionAnimator : IMovementAnimator, IJumpAnimator
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
