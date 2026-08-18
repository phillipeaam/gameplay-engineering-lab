using System.Threading;
using DesignPatterns.Command.Scripts.InitialCommandStudy;
using UnityEngine;

namespace DesignPatterns.Command.Tests.EditMode.TestDoubles
{
    public class FakeActor : IMoveable
    {
        private Vector2 _currentPosition = Vector2.zero;

        public async Awaitable MoveAsync(Vector2 position, CancellationToken cancellationToken)
        {
            _currentPosition = position;
        }

        public Vector2 GetPosition()
        {
            return _currentPosition;
        }
    }
}