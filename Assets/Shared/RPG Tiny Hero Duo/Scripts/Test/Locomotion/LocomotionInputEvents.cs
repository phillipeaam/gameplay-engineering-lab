using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Test.Locomotion
{
    public sealed class LocomotionInputEvents
    {
        private readonly InputAction _moveAction;
        private readonly InputAction _jumpAction;
        private readonly InputAction _lookAction;
        

        private bool _isEnabled;

        public event Action<Vector2> MovementChanged;
        public event Action<Vector2> LookChanged;
        public event Action JumpRequested;

        public LocomotionInputEvents(ILocomotionInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            _moveAction = input.Move;
            _jumpAction = input.Jump;
            _lookAction = input.Look;
        }

        public void Enable()
        {
            if (_isEnabled)
            {
                return;
            }

            _moveAction.performed += OnMovePerformed;
            _moveAction.canceled += OnMoveCanceled;
            _jumpAction.performed += OnJumpPerformed;
            _lookAction.performed += OnLookPerformed;
            _lookAction.canceled += OnLookCanceled;

            _moveAction.Enable();
            _jumpAction.Enable();
            _lookAction.Enable();
            _isEnabled = true;
        }

        public void Disable()
        {
            if (!_isEnabled)
            {
                return;
            }

            _moveAction.performed -= OnMovePerformed;
            _moveAction.canceled -= OnMoveCanceled;
            _jumpAction.performed -= OnJumpPerformed;
            _lookAction.performed -= OnLookPerformed;
            _lookAction.canceled -= OnLookCanceled;

            _moveAction.Disable();
            _jumpAction.Disable();
            _lookAction.Disable();
            _isEnabled = false;
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

        private void OnLookPerformed(InputAction.CallbackContext context)
        {
            LookChanged?.Invoke(context.ReadValue<Vector2>());
        }

        private void OnLookCanceled(InputAction.CallbackContext context)
        {
            LookChanged?.Invoke(Vector2.zero);
        }
    }
}
