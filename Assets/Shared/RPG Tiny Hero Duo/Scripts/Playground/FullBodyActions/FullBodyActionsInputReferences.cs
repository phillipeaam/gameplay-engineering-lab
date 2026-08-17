using System;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.FullBodyActions
{
    [Serializable]
    public sealed class FullBodyActionsInputReferences : IFullBodyActionsInput
    {
        [SerializeField] private InputActionReference _fall;
        [SerializeField] private InputActionReference _stun;
        [SerializeField] private InputActionReference _hitReaction;
        [SerializeField] private InputActionReference _death;
        [SerializeField] private InputActionReference _loop;

        public InputAction Fall => _fall.RequireAction(
            nameof(_fall),
            nameof(FullBodyActionsInputReferences));

        public InputAction Stun => _stun.RequireAction(
            nameof(_stun),
            nameof(FullBodyActionsInputReferences));

        public InputAction HitReaction => _hitReaction.RequireAction(
            nameof(_hitReaction),
            nameof(FullBodyActionsInputReferences));

        public InputAction Death => _death.RequireAction(
            nameof(_death),
            nameof(FullBodyActionsInputReferences));

        public InputAction Loop => _loop.RequireAction(
            nameof(_loop),
            nameof(FullBodyActionsInputReferences));
    }
}
