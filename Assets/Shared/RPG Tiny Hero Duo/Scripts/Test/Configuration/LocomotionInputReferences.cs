using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Test.Configuration
{
    [Serializable]
    public sealed class LocomotionInputReferences : ILocomotionInput
    {
        [SerializeField] private InputActionReference _move;
        [SerializeField] private InputActionReference _look;
        [SerializeField] private InputActionReference _jump;

        public bool IsConfigured => 
            _move != null && 
            _look != null && 
            _jump != null;

        public InputAction Move => _move.action;
        public InputAction Look => _look.action;
        public InputAction Jump => _jump.action;
    }
}
