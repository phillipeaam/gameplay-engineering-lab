using System.Threading;
using UnityEngine;

namespace DesignPatterns.Command.Scripts.Study
{
    public class MoveCommand : ICommand
    {
        private readonly IMoveable _moveable;
        private readonly Vector2 _nextPosition;

        public MoveCommand(IMoveable moveable, Vector2 nextPosition)
        {
            _moveable = moveable;
            _nextPosition = nextPosition;
        }

        public Awaitable ExecuteAsync(CancellationToken cancellationToken)
        {
            return _moveable.MoveAsync(_nextPosition, cancellationToken);
        }
    }
}