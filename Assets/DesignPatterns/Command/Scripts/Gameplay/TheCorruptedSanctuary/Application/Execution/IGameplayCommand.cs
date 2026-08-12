using System.Threading;
using UnityEngine;

namespace DesignPatterns.Command.Scripts.Gameplay.TheCorruptedSanctuary.Application.Execution
{
    public interface IGameplayCommand
    {
        Awaitable ExecuteAsync(CancellationToken cancellationToken);
    }
}
