using Shared.RPG_Tiny_Hero_Duo.Scripts.Locomotion;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion;
using UnityEngine;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Playground
{
    [RequireComponent(typeof(Animator), typeof(CharacterController))]
    public sealed class HeroAnimationPlaygroundController : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private LocomotionInputReferences _locomotionInput;
        
        private LocomotionModule _locomotion;

        private void Awake()
        {
            var animator = GetComponent<Animator>();
            var characterController = GetComponent<CharacterController>();
            var locomotionAnimator = new LocomotionAnimatorAdapter(animator);

            _locomotion = new LocomotionModule(
                characterController,
                locomotionAnimator,
                locomotionAnimator,
                _locomotionInput,
                _locomotionInput,
                _locomotionInput,
                _locomotionInput);
        }

        private void OnEnable()
        {
            _locomotion?.Enable();
        }

        private void OnDisable()
        {
            _locomotion?.Disable();
        }

        private void Update()
        {
            _locomotion?.Tick(Time.deltaTime);
        }
    }
}
