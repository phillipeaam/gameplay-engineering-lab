using NUnit.Framework;
using RPG_Tiny_Hero_Duo.Playground.Locomotion;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG_Tiny_Hero_Duo.Playground.Tests.PlayMode
{
    public sealed class LocomotionModuleValidationTests : LocomotionModuleTestFixture
    {
        /// <summary>
        /// Verifies that locomotion rejects a negative jump height before it can produce
        /// an invalid vertical velocity in the physics calculation.
        /// </summary>
        [Test]
        public void Constructor_WithNegativeJumpHeight_IdentifiesInvalidProperty()
        {
            var configuration = new TestLocomotionConfiguration { JumpHeight = -1f };

            var exception = Assert.Throws<System.ArgumentOutOfRangeException>(
                () => CreateLocomotion(configuration));

            Assert.That(exception.ParamName, Is.EqualTo(nameof(configuration.JumpHeight)));
        }

        /// <summary>
        /// Verifies that locomotion rejects a non-finite movement speed before it can be
        /// forwarded to the character controller.
        /// </summary>
        [Test]
        public void Constructor_WithNonFiniteMovementSpeed_IdentifiesInvalidProperty()
        {
            var configuration = new TestLocomotionConfiguration { MovementSpeed = float.NaN };

            var exception = Assert.Throws<System.ArgumentOutOfRangeException>(
                () => CreateLocomotion(configuration));

            Assert.That(exception.ParamName, Is.EqualTo(nameof(configuration.MovementSpeed)));
        }

        /// <summary>
        /// Verifies that locomotion rejects a landing movement multiplier outside its
        /// normalized range before composing the module.
        /// </summary>
        [Test]
        public void Constructor_WithLandingMultiplierAboveOne_IdentifiesInvalidProperty()
        {
            var configuration = new TestLocomotionConfiguration { LandingMovementMultiplier = 1.1f };

            var exception = Assert.Throws<System.ArgumentOutOfRangeException>(
                () => CreateLocomotion(configuration));

            Assert.That(exception.ParamName, Is.EqualTo(nameof(configuration.LandingMovementMultiplier)));
        }

        /// <summary>
        /// Verifies that an incomplete external input contract is rejected during
        /// composition instead of failing later when locomotion is enabled.
        /// </summary>
        [Test]
        public void Constructor_WithMissingJumpAction_IdentifiesInvalidProperty()
        {
            Character = new GameObject(nameof(LocomotionModuleValidationTests));
            var characterController = Character.AddComponent<CharacterController>();
            var animator = new RecordingLocomotionAnimator();
            var configuration = new TestLocomotionConfiguration();
            Input = new TestLocomotionInput(includeJumpAction: false);

            var exception = Assert.Throws<System.InvalidOperationException>(
                () => new LocomotionModule(
                    characterController,
                    animator,
                    animator,
                    Input,
                    configuration,
                    configuration,
                    configuration));

            Assert.That(exception.Message, Does.Contain(nameof(Input.Jump)));
        }

        /// <summary>
        /// Verifies that the public tick boundary rejects time values that could reverse
        /// simulation progress or propagate non-finite values into movement physics.
        /// </summary>
        [TestCase(-0.1f)]
        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        public void Tick_WithInvalidDeltaTime_IdentifiesInvalidParameter(float invalidDeltaTime)
        {
            CreateLocomotion(out _);

            var exception = Assert.Throws<System.ArgumentOutOfRangeException>(
                () => Locomotion.Tick(invalidDeltaTime));

            Assert.That(exception.ParamName, Is.EqualTo("deltaTime"));
        }

        /// <summary>
        /// Verifies that an invalid gravity result is rejected before jump state is stored
        /// or the jump animation is requested.
        /// </summary>
        [Test]
        public void Tick_WhenGravityProducesInvalidJumpVelocity_RejectsBeforeAnimation()
        {
            var gamepad = InputSystem.AddDevice<Gamepad>();
            var originalGravity = Physics.gravity;
            CreateLocomotion(out var animator);
            Locomotion.Enable();

            try
            {
                Physics.gravity = Vector3.up;
                Press(gamepad.buttonSouth);

                var exception = Assert.Throws<System.InvalidOperationException>(
                    () => Locomotion.Tick(0f));

                Assert.That(exception.Message, Does.Contain("non-finite jump velocity"));
                Assert.That(animator.JumpRequestCount, Is.Zero);
            }
            finally
            {
                Physics.gravity = originalGravity;
            }
        }
    }
}
