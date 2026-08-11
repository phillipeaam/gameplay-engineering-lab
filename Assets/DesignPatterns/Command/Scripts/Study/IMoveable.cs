using System.Threading;
using UnityEngine;

namespace DesignPatterns.Command.Scripts.Study
{
    public interface IMoveable
    {
        Awaitable MoveAsync(Vector2 position, CancellationToken cancellationToken);
        Vector2 GetPosition();
    }
}