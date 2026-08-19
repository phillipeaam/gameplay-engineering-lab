using NUnit.Framework;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion;
using UnityEngine;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Tests.PlayMode
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
    }
}
