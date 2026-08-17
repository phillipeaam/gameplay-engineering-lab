using UnityEngine;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion.Movement
{
    internal sealed class CharacterControllerMotor
    {
        private readonly CharacterController _characterController;
        private readonly ILocomotionConfiguration _configuration;

        public bool IsGrounded => _characterController.isGrounded;

        public CharacterControllerMotor(CharacterController characterController, ILocomotionConfiguration configuration)
        {
            _characterController = characterController;
            _configuration = configuration;
        }

        public CollisionFlags Move(
            Vector2 movementInput,
            float verticalVelocity,
            float movementMultiplier,
            float deltaTime)
        {
            var localInputDirection = new Vector3(movementInput.x, 0f, movementInput.y);
            var localMovementVelocity = localInputDirection * (_configuration.MovementSpeed * movementMultiplier);
            var worldMovementVelocity = _characterController.transform.TransformDirection(localMovementVelocity);

            worldMovementVelocity.y = verticalVelocity;

            return _characterController.Move(worldMovementVelocity * deltaTime);
        }

        public void Rotate(Vector2 lookInput, float deltaTime)
        {
            var yAngle = lookInput.x * _configuration.RotationSpeed * deltaTime;
            _characterController.transform.Rotate(Vector3.up, yAngle, Space.World);
        }
    }
}
