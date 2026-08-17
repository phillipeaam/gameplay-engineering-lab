using System;
using Shared.RPG_Tiny_Hero_Duo.Scripts.FullBodyActions;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Shared;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.FullBodyActions
{
    public sealed class FullBodyActionsModule : IAnimationModule
    {
        private readonly IFullBodyActionAnimator _animator;
        private readonly FullBodyActionInputEvents _inputEvents;
        private bool _isEnabled;

        public FullBodyActionsModule(IFullBodyActionAnimator animator, IFullBodyActionsInput input)
        {
            _animator = animator ?? throw new ArgumentNullException(nameof(animator));

            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            _inputEvents = new FullBodyActionInputEvents(input);
        }

        public void Enable()
        {
            if (_isEnabled)
            {
                return;
            }

            _inputEvents.ActionRequested += OnActionRequested;
            _inputEvents.LoopReleased += OnLoopReleased;
            _inputEvents.Enable();
            _isEnabled = true;
        }

        public void Disable()
        {
            if (!_isEnabled)
            {
                return;
            }

            _inputEvents.ActionRequested -= OnActionRequested;
            _inputEvents.LoopReleased -= OnLoopReleased;
            _inputEvents.Disable();
            _isEnabled = false;
        }

        private void OnActionRequested(FullBodyAction action, bool isLooped)
        {
            if (isLooped)
            {
                _animator.PlayLoop(action);
                return;
            }

            _animator.PlayOnce(action);
        }

        private void OnLoopReleased()
        {
            _animator.Reset();
        }
    }
}
