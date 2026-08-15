using System;
using Shared.RPG_Tiny_Hero_Duo.Scripts.FullBodyActions;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Test.Configuration;
using UnityEngine.InputSystem;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Test.Input
{
    public sealed class FullBodyActionInputEvents
    {
        private readonly InputAction _fallAction;
        private readonly InputAction _stunAction;
        private readonly InputAction _hitReactionAction;
        private readonly InputAction _deathAction;

        public event Action<FullBodyAction> ActionRequested;

        public FullBodyActionInputEvents(
            IFullBodyActionsInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            _fallAction = input.Fall ?? throw new ArgumentNullException(nameof(input.Fall));
            _stunAction = input.Stun ?? throw new ArgumentNullException(nameof(input.Stun));
            _hitReactionAction = input.HitReaction ?? throw new ArgumentNullException(nameof(input.HitReaction));
            _deathAction = input.Death ?? throw new ArgumentNullException(nameof(input.Death));
        }

        public void Enable()
        {
            _fallAction.performed += OnFallPerformed;
            _stunAction.performed += OnStunPerformed;
            _hitReactionAction.performed += OnHitReactionPerformed;
            _deathAction.performed += OnDeathPerformed;

            _fallAction.Enable();
            _stunAction.Enable();
            _hitReactionAction.Enable();
            _deathAction.Enable();
        }

        public void Disable()
        {
            _fallAction.performed -= OnFallPerformed;
            _stunAction.performed -= OnStunPerformed;
            _hitReactionAction.performed -= OnHitReactionPerformed;
            _deathAction.performed -= OnDeathPerformed;

            _fallAction.Disable();
            _stunAction.Disable();
            _hitReactionAction.Disable();
            _deathAction.Disable();
        }

        private void OnFallPerformed(InputAction.CallbackContext context) => Request(FullBodyAction.Fall);
        private void OnStunPerformed(InputAction.CallbackContext context) => Request(FullBodyAction.Stun);
        private void OnHitReactionPerformed(InputAction.CallbackContext context) => Request(FullBodyAction.HitReaction);
        private void OnDeathPerformed(InputAction.CallbackContext context) => Request(FullBodyAction.Death);

        private void Request(FullBodyAction action)
        {
            ActionRequested?.Invoke(action);
        }
    }
}
