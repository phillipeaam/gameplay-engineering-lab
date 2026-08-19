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
            _characterController = characterController.RequireValid();
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

            return _characterController.Move(worldMovementVelocity * deltaTime);
        }

        public void Rotate(Vector2 lookInput, float deltaTime)
        {
            var rotationSpeed = _settings.GetValidatedRotationSpeed();
            var yAngle = lookInput.x * rotationSpeed * deltaTime;
            _characterController.transform.Rotate(Vector3.up, yAngle, Space.World);
        }
    }
}
