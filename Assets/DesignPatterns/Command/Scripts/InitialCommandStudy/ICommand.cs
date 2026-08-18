using System.Threading;
using UnityEngine;

namespace DesignPatterns.Command.Scripts.InitialCommandStudy
{
    public interface ICommand
    {
        Awaitable ExecuteAsync(CancellationToken cancellationToken);
    }
}