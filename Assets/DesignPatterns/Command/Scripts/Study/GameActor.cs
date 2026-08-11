using System;
using System.Threading;
using UnityEngine;

namespace DesignPatterns.Command.Scripts.Study
{
    public class GameActor : MonoBehaviour, IMoveable
    {
        [SerializeField] private float _movementDuration = 1f;

        private Vector2 _cubePosition;

        private void Awake()
        {
            _cubePosition = new Vector2(transform.position.x, transform.position.z);
        }

        public async Awaitable MoveAsync(Vector2 position, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Keeps the y-axis in place
            var startPosition = transform.position;
            var targetPosition = new Vector3(position.x, startPosition.y, position.y);

            Debug.Log($"Cube - Leaving position {startPosition} and moving to {targetPosition}");

            if (_movementDuration > 0)
            {
                var elapsedTime = 0f;
                var distance = Vector3.Distance(startPosition, targetPosition);
                var speed = distance / _movementDuration;

                while (elapsedTime < _movementDuration)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    transform.position = Vector3.MoveTowards(
                        transform.position,
                        targetPosition,
                        speed * Time.deltaTime
                    );

                    elapsedTime += Time.deltaTime;
                    
                    await Awaitable.NextFrameAsync(cancellationToken);
                }
            }

            transform.position = targetPosition;
            _cubePosition = position;
        }

        public Vector2 GetPosition()
        {
            return _cubePosition;
        }

        private CancellationTokenSource _testMovementCancellation;

        [ContextMenu("Test Movement")]
        public async void TestMovement()
        {
            _testMovementCancellation?.Cancel();

            var cancellation = CancellationTokenSource.CreateLinkedTokenSource(
                destroyCancellationToken
            );

            _testMovementCancellation = cancellation;

            try
            {
                var token = cancellation.Token;

                var commands = new[]
                {
                    new MoveCommand(this, Vector2.right * 5),
                    new MoveCommand(this, Vector2.left * 5),
                    new MoveCommand(this, Vector2.right * 2)
                };

                await CommandProcessor.ProcessCommandsAsync(commands, token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Movement cancelled.");
            }
            finally
            {
                if (_testMovementCancellation == cancellation)
                {
                    _testMovementCancellation = null;
                }

                cancellation.Dispose();
            }
        }
    }
}