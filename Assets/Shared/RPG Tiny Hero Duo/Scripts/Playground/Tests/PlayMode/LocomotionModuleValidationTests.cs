using NUnit.Framework;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion;
using UnityEngine;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Tests.PlayMode
{
    public sealed class LocomotionModuleValidationTests : LocomotionModuleTestFixture
    {
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
