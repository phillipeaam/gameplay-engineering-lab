using NUnit.Framework;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion.Grounding;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion.Jumping;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion.Landing;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Tests.PlayMode
{
    public sealed class LocomotionModuleBehaviorTests : LocomotionModuleTestFixture
    {
        /// <summary>
        /// Verifies that movement input drives both horizontal character displacement and
        /// the movement animation contract during a locomotion tick.
        /// </summary>
        [Test]
        public void Tick_WithMovementInput_MovesCharacterAndUpdatesAnimation()
        {
            var gamepad = InputSystem.AddDevice<Gamepad>();
            CreateLocomotion(out var animator);
            Locomotion.Enable();
            Set(gamepad.leftStick, Vector2.up);

            Locomotion.Tick(0.1f);

            Assert.That(Character.transform.position.z, Is.EqualTo(0.5f).Within(0.0001f));
            Assert.That(animator.LastMovement, Is.EqualTo(Vector2.up));
        }

        /// <summary>
        /// Verifies that look input rotates the character by the configured angular speed
        /// and elapsed frame time.
        /// </summary>
        [Test]
        public void Tick_WithLookInput_RotatesCharacterAtConfiguredSpeed()
        {
            var gamepad = InputSystem.AddDevice<Gamepad>();
            CreateLocomotion(out _);
            Locomotion.Enable();
            Set(gamepad.rightStick, Vector2.right);

            Locomotion.Tick(0.1f);

            Assert.That(Character.transform.eulerAngles.y, Is.EqualTo(25f).Within(0.0001f));
        }

        /// <summary>
        /// Verifies that a grounded jump calculates upward velocity and requests its
        /// animation once.
        /// </summary>
        [Test]
        public void ProcessRequest_WhileGrounded_AppliesJumpVelocityAndAnimation()
        {
            var configuration = new TestLocomotionConfiguration();
            var animator = new RecordingLocomotionAnimator();
            var controller = new JumpController(configuration, animator);

            controller.RequestJump();

            controller.ProcessRequest(isGrounded: true, isJumpBlocked: false);

            var expectedVelocity = Mathf.Sqrt(configuration.JumpHeight * -2f * Physics.gravity.y);

            Assert.That(controller.VerticalVelocity, Is.EqualTo(expectedVelocity).Within(0.0001f));
            Assert.That(animator.JumpRequestCount, Is.EqualTo(1));
        }

        /// <summary>
        /// Verifies that an enabled double jump grants exactly one additional airborne
        /// jump before the next grounded transition.
        /// </summary>
        [Test]
        public void ProcessRequest_WhileAirborne_ConsumesSingleDoubleJump()
        {
            var configuration = new TestLocomotionConfiguration { CanDoubleJump = true };
            var animator = new RecordingLocomotionAnimator();
            var controller = new JumpController(configuration, animator);

            controller.RequestJump();
            controller.ProcessRequest(isGrounded: true, isJumpBlocked: false);

            controller.RequestJump();
            controller.ProcessRequest(isGrounded: false, isJumpBlocked: false);

            controller.RequestJump();
            controller.ProcessRequest(isGrounded: false, isJumpBlocked: false);

            Assert.That(animator.JumpRequestCount, Is.EqualTo(2));
        }

        /// <summary>
        /// Verifies that grounding reports a landing only when contact follows an airborne
        /// state, rather than on initial grounded composition.
        /// </summary>
        [Test]
        public void Update_AfterAirborneState_ReportsLandingTransition()
        {
            var tracker = new GroundedStateTracker();
            tracker.Update(CollisionFlags.None);

            var stateChange = tracker.Update(CollisionFlags.Below);

            Assert.That(stateChange.IsGrounded, Is.True);
            Assert.That(stateChange.HasChanged, Is.True);
            Assert.That(stateChange.HasLanded, Is.True);
        }

        /// <summary>
        /// Verifies that landing recovery interpolates movement from its configured initial
        /// restriction back to full speed over the configured duration.
        /// </summary>
        [Test]
        public void Tick_DuringLandingRecovery_RestoresFullMovementMultiplier()
        {
            var configuration = new TestLocomotionConfiguration
            {
                LandingDuration = 0.4f,
                LandingMovementMultiplier = 0.2f
            };

            var recovery = new LandingRecovery(configuration);

            recovery.Start();

            Assert.That(recovery.GetMovementMultiplier(), Is.EqualTo(0.2f).Within(0.0001f));

            recovery.Tick(0.2f);

            Assert.That(recovery.GetMovementMultiplier(), Is.EqualTo(0.6f).Within(0.0001f));

            recovery.Tick(0.2f);

            Assert.That(recovery.GetMovementMultiplier(), Is.EqualTo(1f).Within(0.0001f));
            Assert.That(recovery.IsActive, Is.False);
        }
    }
}
