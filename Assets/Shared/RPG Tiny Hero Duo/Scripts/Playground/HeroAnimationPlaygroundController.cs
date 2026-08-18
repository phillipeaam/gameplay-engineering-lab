using System;
using Shared.RPG_Tiny_Hero_Duo.Scripts.FullBodyActions;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Locomotion;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.FullBodyActions;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion;
using UnityEngine;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Playground
{
    [RequireComponent(typeof(Animator), typeof(CharacterController))]
    public sealed class HeroAnimationPlaygroundController : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private LocomotionInputReferences _locomotionInput;
        [SerializeField] private FullBodyActionsInputReferences _fullBodyActionsInput;
        
        private LocomotionModule _locomotion;
        private FullBodyActionsModule _fullBodyActions;

        private void Awake()
        {
            ValidateInputReferences();
            
            var characterController = GetComponent<CharacterController>();
            
            var animator = GetComponent<Animator>();

            _locomotion = new LocomotionModule(
                characterController,
                new LocomotionAnimatorAdapter(animator),
                _locomotionInput,
                _locomotionInput,
                _locomotionInput);

            _fullBodyActions = new FullBodyActionsModule(
                new FullBodyActionAnimatorAdapter(animator),
                _fullBodyActionsInput);
        }

        private void OnEnable()
        {
            _locomotion?.Enable();
            _fullBodyActions?.Enable();
        }

        private void OnDisable()
        {
            _locomotion?.Disable();
            _fullBodyActions?.Disable();
        }

        private void Update()
        {
            _locomotion?.Tick(Time.deltaTime);
        }

        private void ValidateInputReferences()
        {
            if (_locomotionInput == null || _fullBodyActionsInput == null)
            {
                throw new InvalidOperationException(
                    "HeroAnimationPlaygroundController requires input configurations.");
            }
        }
    }
}
