using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Test.FullBodyActions
{
    [Serializable]
    public sealed class FullBodyActionsInputReferences : IFullBodyActionsInput
    {
        [SerializeField] private InputActionReference _fall;
        [SerializeField] private InputActionReference _stun;
        [SerializeField] private InputActionReference _hitReaction;
        [SerializeField] private InputActionReference _death;
        [SerializeField] private InputActionReference _loop;

        public InputAction Fall => _fall.action;
        public InputAction Stun => _stun.action;
        public InputAction HitReaction => _hitReaction.action;
        public InputAction Death => _death.action;
        public InputAction Loop => _loop.action;
    }
}
