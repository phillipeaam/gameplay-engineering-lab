using System.Threading;
using UnityEngine;

namespace DesignPatterns.Command.Scripts.Study
{
    public interface ICommand
    {
        Awaitable ExecuteAsync(CancellationToken cancellationToken);
    }
}