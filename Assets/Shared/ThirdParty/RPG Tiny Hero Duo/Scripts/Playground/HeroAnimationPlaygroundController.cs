using System;
using ModuleHosting.Scripts;
using RPG_Tiny_Hero_Duo.Locomotion;
using RPG_Tiny_Hero_Duo.Playground.Locomotion;
using UnityEngine;

namespace RPG_Tiny_Hero_Duo.Playground
{
    [RequireComponent(typeof(Animator), typeof(CharacterController))]
    public sealed class HeroAnimationPlaygroundController : MonoBehaviour
    {
        private readonly ILogger _logger = Debug.unityLogger;

        [Header("Input")]
        [SerializeField] private LocomotionInputReferences _locomotionInput;

        private ModuleHost<LocomotionModule> _locomotion;

        private void Awake()
        {
            try
            {
                _locomotion = new ModuleHost<LocomotionModule>(CreateLocomotion, OnLocomotionFailed, _logger);
            }
            catch (Exception e)
            {
                _logger.LogException(e);
            }

            _locomotion.Compose();
        }

        private void OnEnable()
        {
            _locomotion.Enable();
        }

        private void OnDisable()
        {
            _locomotion.Disable();
        }

        private void Update()
        {
            _locomotion.Tick(Time.deltaTime);
        }

        private LocomotionModule CreateLocomotion()
        {
            var animator = GetComponent<Animator>();
            var characterController = GetComponent<CharacterController>();

            var locomotionAnimator = new LocomotionAnimatorAdapter(animator);

            return new LocomotionModule(
                characterController,
                locomotionAnimator,
                locomotionAnimator,
                _locomotionInput,
                _locomotionInput,
                _locomotionInput,
                _locomotionInput);
        }

        private void OnLocomotionFailed(ModuleFailure failure)
        {
            _logger.LogError(
                $"Locomotion failed during {failure.Stage}. " +
                $"{nameof(HeroAnimationPlaygroundController)} has been disabled.",
                this);

            _logger.LogException(failure.Exception, this);
        }
    }
}
