using System;
using UnityEngine;

namespace RPG_Tiny_Hero_Duo.Locomotion
{
    public sealed class LocomotionAnimatorAdapter : IMovementAnimator, IJumpAnimator
    {
        private static readonly int MoveX = Animator.StringToHash("MoveX");
        private static readonly int MoveY = Animator.StringToHash("MoveY");
        private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
        private static readonly int JumpRequest = Animator.StringToHash("JumpRequest");

        private readonly Animator _animator;

        public LocomotionAnimatorAdapter(Animator animator)
        {
            _animator = animator ?? throw new ArgumentNullException(nameof(animator));
        }

        public void ApplyMovement(Vector2 movement)
        {
            _animator.SetFloat(MoveX, movement.x);
            _animator.SetFloat(MoveY, movement.y);
        }

        public void SetGrounded(bool isGrounded)
        {
            _animator.SetBool(IsGrounded, isGrounded);
        }

        public void RequestJump()
        {
            _animator.SetTrigger(JumpRequest);
        }
    }
}
