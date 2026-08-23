using System;
using RPG_Tiny_Hero_Duo.Playground.Extensions;
using RPG_Tiny_Hero_Duo.Playground.Locomotion.Input;
using RPG_Tiny_Hero_Duo.Playground.Locomotion.Jumping;
using RPG_Tiny_Hero_Duo.Playground.Locomotion.Landing;
using RPG_Tiny_Hero_Duo.Playground.Locomotion.Movement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG_Tiny_Hero_Duo.Playground.Locomotion
{
    [Serializable]
    public sealed class LocomotionInputReferences :
        ILocomotionInput,
        IMovementSettings,
        IJumpSettings,
        ILandingSettings
    {
        [Tooltip("Input action used for horizontal and vertical movement.")]
        [SerializeField] private InputActionReference _move;

        [Tooltip("Input action used to rotate the character horizontally.")]
        [SerializeField] private InputActionReference _look;

        [Tooltip("Input action used to request a jump.")]
        [SerializeField] private InputActionReference _jump;

        [Tooltip("Maximum horizontal movement speed in units per second.")]
        [Min(0f)]
        [SerializeField] private float _movementSpeed = 5f;

        [Tooltip("Character rotation speed in degrees per second.")]
        [Min(0f)]
        [SerializeField] private float _rotationSpeed = 250f;

        [Tooltip("Maximum vertical height reached by a jump, in world units.")]
        [Min(0f)]
        [SerializeField] private float _jumpHeight = 1.5f;

        [Tooltip("Allows the character to perform one additional jump while airborne.")]
        [SerializeField] private bool _canDoubleJump = true;

        [Tooltip("How long jumping remains locked after landing, in seconds.")]
        [Min(0f)]
        [SerializeField] private float _landingDuration = 0.4f;

        [Range(0f, 1f)]
        [Tooltip("Movement speed multiplier immediately after landing. 0 means no movement and 1 means full speed.")]
        [SerializeField] private float _landingMovementMultiplier = 0.2f;


        public InputAction Move => _move.RequireAction(
            nameof(_move),
            nameof(LocomotionInputReferences));

        public InputAction Look => _look.RequireAction(
            nameof(_look),
            nameof(LocomotionInputReferences));

        public InputAction Jump => _jump.RequireAction(
            nameof(_jump),
            nameof(LocomotionInputReferences));

        public float MovementSpeed => _movementSpeed.RequireNonNegativeFinite(
            nameof(_movementSpeed),
            nameof(LocomotionInputReferences));

        public float RotationSpeed => _rotationSpeed.RequireNonNegativeFinite(
            nameof(_rotationSpeed),
            nameof(LocomotionInputReferences));

        public float JumpHeight => _jumpHeight.RequireNonNegativeFinite(
            nameof(_jumpHeight),
            nameof(LocomotionInputReferences));

        public bool CanDoubleJump => _canDoubleJump;

        public float LandingDuration => _landingDuration.RequireNonNegativeFinite(
            nameof(_landingDuration),
            nameof(LocomotionInputReferences));

        public float LandingMovementMultiplier => _landingMovementMultiplier.RequireNormalizedFinite(
            nameof(_landingMovementMultiplier),
            nameof(LocomotionInputReferences));
    }
}
