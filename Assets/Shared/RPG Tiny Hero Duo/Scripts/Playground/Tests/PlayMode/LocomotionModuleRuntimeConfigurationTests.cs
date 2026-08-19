using NUnit.Framework;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion.Landing;
using UnityEngine.InputSystem;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Tests.PlayMode
{
    public sealed class LocomotionModuleRuntimeConfigurationTests : LocomotionModuleTestFixture
    {
        /// <summary>
        /// Verifies that movement speed is revalidated when consumed so runtime mutation
        /// cannot reach CharacterController.Move with an invalid value.
        /// </summary>
        [Test]
        public void Tick_WhenMovementSpeedBecomesInvalid_IdentifiesInvalidProperty()
        {
            var configuration = new TestLocomotionConfiguration();
            CreateLocomotion(configuration);
            configuration.MovementSpeed = float.NaN;

            var exception = Assert.Throws<System.ArgumentOutOfRangeException>(
                () => Locomotion.Tick(0f));

            Assert.That(exception.ParamName, Is.EqualTo(nameof(configuration.MovementSpeed)));
        }

        /// <summary>
        /// Verifies that rotation speed is revalidated when consumed so runtime mutation
        /// cannot produce an invalid transform rotation after successful composition.
        /// </summary>
        [Test]
        public void Tick_WhenRotationSpeedBecomesInvalid_IdentifiesInvalidProperty()
        {
            var configuration = new TestLocomotionConfiguration();
            CreateLocomotion(configuration);
            configuration.RotationSpeed = float.PositiveInfinity;

            var exception = Assert.Throws<System.ArgumentOutOfRangeException>(
                () => Locomotion.Tick(0f));

            Assert.That(exception.ParamName, Is.EqualTo(nameof(configuration.RotationSpeed)));
        }

        /// <summary>
        /// Verifies that jump height is revalidated at the jump decision so runtime
        /// mutation cannot feed a negative value into the square-root calculation.
        /// </summary>
        [Test]
        public void Tick_WhenJumpHeightBecomesInvalid_IdentifiesInvalidProperty()
        {
            var gamepad = InputSystem.AddDevice<Gamepad>();
            var configuration = new TestLocomotionConfiguration();
            CreateLocomotion(configuration);
            Locomotion.Enable();
            configuration.JumpHeight = -1f;
            Press(gamepad.buttonSouth);

            var exception = Assert.Throws<System.ArgumentOutOfRangeException>(
                () => Locomotion.Tick(0f));

            Assert.That(exception.ParamName, Is.EqualTo(nameof(configuration.JumpHeight)));
        }

        /// <summary>
        /// Verifies that landing duration is revalidated when recovery starts so a runtime
        /// mutation cannot create an invalid recovery timer after successful composition.
        /// </summary>
        [Test]
        public void StartLandingRecovery_WhenLandingDurationBecomesInvalid_IdentifiesInvalidProperty()
        {
            var configuration = new TestLocomotionConfiguration();
            var landingRecovery = new LandingRecovery(configuration);
            configuration.LandingDuration = float.NaN;

            var exception = Assert.Throws<System.ArgumentOutOfRangeException>(
                landingRecovery.Start);

            Assert.That(exception.ParamName, Is.EqualTo(nameof(configuration.LandingDuration)));
        }

        /// <summary>
        /// Verifies that the landing movement multiplier is revalidated when recovery starts
        /// so a runtime mutation cannot enter the recovery interpolation after composition.
        /// </summary>
        [Test]
        public void StartLandingRecovery_WhenMovementMultiplierBecomesInvalid_IdentifiesInvalidProperty()
        {
            var configuration = new TestLocomotionConfiguration();
            var landingRecovery = new LandingRecovery(configuration);
            configuration.LandingMovementMultiplier = 1.1f;

            var exception = Assert.Throws<System.ArgumentOutOfRangeException>(
                landingRecovery.Start);

            Assert.That(
                exception.ParamName,
                Is.EqualTo(nameof(configuration.LandingMovementMultiplier)));
        }
    }
}
