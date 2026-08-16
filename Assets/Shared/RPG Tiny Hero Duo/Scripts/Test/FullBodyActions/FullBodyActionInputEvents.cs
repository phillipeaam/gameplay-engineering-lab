using System;
using Shared.RPG_Tiny_Hero_Duo.Scripts.FullBodyActions;
using UnityEngine.InputSystem;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Test.FullBodyActions
{
    public sealed class FullBodyActionInputEvents
    {
        private readonly InputAction _fallAction;
        private readonly InputAction _stunAction;
        private readonly InputAction _hitReactionAction;
        private readonly InputAction _deathAction;
        private readonly InputAction _loopAction;

        private bool _isEnabled;

        public event Action<FullBodyAction, bool> ActionRequested;
        public event Action LoopReleased;

        public FullBodyActionInputEvents(IFullBodyActionsInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            _fallAction = input.Fall ?? throw new ArgumentNullException(nameof(input.Fall));
            _stunAction = input.Stun ?? throw new ArgumentNullException(nameof(input.Stun));
            _hitReactionAction = input.HitReaction ?? throw new ArgumentNullException(nameof(input.HitReaction));
            _deathAction = input.Death ?? throw new ArgumentNullException(nameof(input.Death));
            _loopAction = input.Loop ?? throw new ArgumentNullException(nameof(input.Loop));
        }

        public void Enable()
        {
            if (_isEnabled) return;
            _fallAction.performed += OnFallPerformed;
            _stunAction.performed += OnStunPerformed;
            _hitReactionAction.performed += OnHitReactionPerformed;
            _deathAction.performed += OnDeathPerformed;
            _loopAction.canceled += OnLoopCanceled;

            _fallAction.Enable();
            _stunAction.Enable();
            _hitReactionAction.Enable();
            _deathAction.Enable();
            _loopAction.Enable();
            _isEnabled = true;
        }

        public void Disable()
        {
            if (!_isEnabled) return;
            _fallAction.performed -= OnFallPerformed;
            _stunAction.performed -= OnStunPerformed;
            _hitReactionAction.performed -= OnHitReactionPerformed;
            _deathAction.performed -= OnDeathPerformed;
            _loopAction.canceled -= OnLoopCanceled;

            _fallAction.Disable();
            _stunAction.Disable();
            _hitReactionAction.Disable();
            _deathAction.Disable();
            _loopAction.Disable();
            _isEnabled = false;
        }

        private void OnFallPerformed(InputAction.CallbackContext context) => Request(FullBodyAction.Fall);
        private void OnStunPerformed(InputAction.CallbackContext context) => Request(FullBodyAction.Stun);
        private void OnHitReactionPerformed(InputAction.CallbackContext context) => Request(FullBodyAction.HitReaction);
        private void OnDeathPerformed(InputAction.CallbackContext context) => Request(FullBodyAction.Death);
        private void OnLoopCanceled(InputAction.CallbackContext context) => LoopReleased?.Invoke();

        private void Request(FullBodyAction action)
        {
            ActionRequested?.Invoke(action, _loopAction.IsPressed());
        }
    }
}
