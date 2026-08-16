using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Test.Locomotion
{
    [Serializable]
    public sealed class LocomotionInputReferences : ILocomotionInput, ILocomotionSettings
    {
        [SerializeField] private InputActionReference _move;
        [SerializeField] private InputActionReference _look;
        [SerializeField] private InputActionReference _jump;
        [SerializeField] private float _movementSpeed = 3f;
        [SerializeField] private float _rotationSpeed = 180f;
        [SerializeField] private float _jumpHeight = 1.5f;
        [SerializeField] private bool _canDoubleJump = true;

        public InputAction Move => _move.action;
        public InputAction Look => _look.action;
        public InputAction Jump => _jump.action;
        public float MovementSpeed => _movementSpeed;
        public float RotationSpeed => _rotationSpeed;
        public float JumpHeight => _jumpHeight;
        public bool CanDoubleJump => _canDoubleJump;
    }
}