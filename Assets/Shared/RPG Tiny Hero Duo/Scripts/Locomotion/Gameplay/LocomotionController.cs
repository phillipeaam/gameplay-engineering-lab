using System;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Locomotion.Animation;
using UnityEngine;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Locomotion.Gameplay
{
    public sealed class LocomotionController
    {
        private readonly IJumpHandler _jumpHandler;
        private readonly ILocomotionAnimator _animator;

        public LocomotionController(
            IJumpHandler jumpHandler,
            ILocomotionAnimator animator)
        {
            _jumpHandler = jumpHandler ?? throw new ArgumentNullException(nameof(jumpHandler));
            _animator = animator ?? throw new ArgumentNullException(nameof(animator));
        }

        public void OnMovementChanged(Vector2 movement)
        {
            _animator.ApplyMovement(movement);
        }

        public void OnJumpRequested()
        {
            if (_jumpHandler.TryJump())
            {
                _animator.RequestJump();
            }
        }

        public void OnGroundedChanged(bool isGrounded)
        {
            _animator.SetGrounded(isGrounded);
        }
    }
}
