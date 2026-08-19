using System;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Extensions;
using UnityEngine;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion.Movement
{
    internal sealed class CharacterControllerMotor
    {
        private readonly CharacterController _characterController;
        private readonly IMovementSettings _settings;

        public bool IsGrounded => _characterController.isGrounded;

        public CharacterControllerMotor(CharacterController characterController, IMovementSettings settings)
        {
            _characterController = characterController
                ?? throw new ArgumentNullException(nameof(characterController));
            _settings = settings.RequireValid();
        }

        public CollisionFlags Move(
            Vector2 movementInput,
            float verticalVelocity,
            float movementMultiplier,
            float deltaTime)
        {
            var localInputDirection = new Vector3(movementInput.x, 0f, movementInput.y);
            var movementSpeed = _settings.GetValidatedMovementSpeed();
            var localMovementVelocity = localInputDirection * (movementSpeed * movementMultiplier);
            var worldMovementVelocity = _characterController.transform.TransformDirection(localMovementVelocity);

            worldMovementVelocity.y = verticalVelocity;

            var displacement = worldMovementVelocity * deltaTime;

            if (!displacement.IsFinite())
            {
                throw new InvalidOperationException(
                    $"{nameof(CharacterControllerMotor)} produced a non-finite displacement. " +
                    $"Movement input: {movementInput}, vertical velocity: {verticalVelocity}, " +
                    $"movement multiplier: {movementMultiplier}, delta time: {deltaTime}, " +
                    $"displacement: {displacement}.");
            }

            return _characterController.Move(displacement);
        }

        public void Rotate(Vector2 lookInput, float deltaTime)
        {
            var rotationSpeed = _settings.GetValidatedRotationSpeed();
            var yAngle = lookInput.x * rotationSpeed * deltaTime;

            if (!yAngle.IsFinite())
            {
                throw new InvalidOperationException(
                    $"{nameof(CharacterControllerMotor)} produced a non-finite rotation angle. " +
                    $"Look input: {lookInput}, rotation speed: {rotationSpeed}, " +
                    $"delta time: {deltaTime}, y angle: {yAngle}.");
            }

            _characterController.transform.Rotate(Vector3.up, yAngle, Space.World);
        }
    }
}
