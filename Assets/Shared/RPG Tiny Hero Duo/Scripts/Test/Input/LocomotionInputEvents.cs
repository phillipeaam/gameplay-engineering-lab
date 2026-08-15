using System;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Test.Configuration;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Test.Input
{
    public sealed class LocomotionInputEvents
    {
        private readonly InputAction _moveAction;
        private readonly InputAction _jumpAction;

        public event Action<Vector2> MovementChanged;
        public event Action JumpRequested;

        public LocomotionInputEvents(ILocomotionInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            _moveAction = input.Move ?? throw new ArgumentNullException(nameof(input.Move));
            _jumpAction = input.Jump ?? throw new ArgumentNullException(nameof(input.Jump));
        }

        public void Enable()
        {
            _moveAction.performed += OnMovePerformed;
            _moveAction.canceled += OnMoveCanceled;
            _jumpAction.performed += OnJumpPerformed;

            _moveAction.Enable();
            _jumpAction.Enable();
        }

        public void Disable()
        {
            _moveAction.performed -= OnMovePerformed;
            _moveAction.canceled -= OnMoveCanceled;
            _jumpAction.performed -= OnJumpPerformed;

            _moveAction.Disable();
            _jumpAction.Disable();
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            MovementChanged?.Invoke(context.ReadValue<Vector2>());
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            MovementChanged?.Invoke(Vector2.zero);
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            JumpRequested?.Invoke();
        }
    }
}
