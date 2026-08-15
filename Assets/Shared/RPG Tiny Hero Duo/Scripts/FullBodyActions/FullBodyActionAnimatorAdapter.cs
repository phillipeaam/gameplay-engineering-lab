using System;
using UnityEngine;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.FullBodyActions
{
    public sealed class FullBodyActionAnimatorAdapter : IFullBodyActionAnimator
    {
        private static readonly int FullBodyActionIndex = Animator.StringToHash("FullBodyActionIndex");
        private static readonly int FullBodyActionActive = Animator.StringToHash("FullBodyActionActive");
        private static readonly int FullBodyActionRequest = Animator.StringToHash("FullBodyActionRequest");

        private readonly Animator _animator;

        public FullBodyActionAnimatorAdapter(Animator animator)
        {
            _animator = animator ?? throw new ArgumentNullException(nameof(animator));
        }

        public void PlayOnce(FullBodyAction action)
        {
            Apply(new FullBodyActionAnimation(action, isActive: false));
        }

        public void PlayLoop(FullBodyAction action)
        {
            Apply(new FullBodyActionAnimation(action, isActive: true));
        }

        public void Reset()
        {
            Apply(new FullBodyActionAnimation(FullBodyAction.None, isActive: false));
        }

        private void Apply(FullBodyActionAnimation animation)
        {
            _animator.SetInteger(FullBodyActionIndex, (int)animation.Action);
            _animator.SetBool(FullBodyActionActive, animation.IsActive);

            if (animation.Action != FullBodyAction.None)
            {
                _animator.SetTrigger(FullBodyActionRequest);
            }
        }
    }
}
