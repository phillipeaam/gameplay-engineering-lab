using System.Threading;
using UnityEngine;

namespace DesignPatterns.Command.Scripts.TheCorruptedSanctuary.Application.Execution
{
    public interface IGameplayCommand
    {
        Awaitable ExecuteAsync(CancellationToken cancellationToken);
    }
}
