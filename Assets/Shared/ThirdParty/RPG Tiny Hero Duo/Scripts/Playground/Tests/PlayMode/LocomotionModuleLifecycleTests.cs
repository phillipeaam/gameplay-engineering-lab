using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG_Tiny_Hero_Duo.Playground.Tests.PlayMode
{
    public sealed class LocomotionModuleLifecycleTests : LocomotionModuleTestFixture
    {
        /// <summary>
        /// Verifies that disabling locomotion cancels a pending jump request so the
        /// character does not jump by itself when locomotion is enabled again.
        /// </summary>
        [Test]
        public void Disable_WithPendingJump_DoesNotJumpAfterReenable()
        {
            var gamepad = InputSystem.AddDevice<Gamepad>();
            CreateLocomotion(out var animator);

            Locomotion.Enable();
            Press(gamepad.buttonSouth);

            Locomotion.Disable();
            Release(gamepad.buttonSouth);
            Locomotion.Enable();
            Locomotion.Tick(0f);

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

            Locomotion.Enable();
            Set(gamepad.leftStick, Vector2.up);
            Set(gamepad.rightStick, Vector2.right);

            Locomotion.Disable();
            Set(gamepad.leftStick, Vector2.zero);
            Set(gamepad.rightStick, Vector2.zero);
            Locomotion.Enable();

            var positionBeforeTick = Character.transform.position;
            var rotationBeforeTick = Character.transform.rotation;
            Locomotion.Tick(1f);

            Assert.That(animator.LastMovement, Is.EqualTo(Vector2.zero));
            Assert.That(Character.transform.position.x, Is.EqualTo(positionBeforeTick.x).Within(0.0001f));
            Assert.That(Character.transform.position.z, Is.EqualTo(positionBeforeTick.z).Within(0.0001f));
            Assert.That(Character.transform.rotation, Is.EqualTo(rotationBeforeTick));
        }
    }
}
