using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Test.Configuration
{
    [Serializable]
    public sealed class FullBodyActionsInputReferences : IFullBodyActionsInput
    {
        [SerializeField] private InputActionReference _fall;
        [SerializeField] private InputActionReference _stun;
        [SerializeField] private InputActionReference _hitReaction;
        [SerializeField] private InputActionReference _death;

        public bool IsConfigured =>
            _fall != null &&
            _stun != null &&
            _hitReaction != null &&
            _death != null;

        public InputAction Fall => _fall.action;
        public InputAction Stun => _stun.action;
        public InputAction HitReaction => _hitReaction.action;
        public InputAction Death => _death.action;
    }
}