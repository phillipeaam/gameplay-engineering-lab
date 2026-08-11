using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace DesignPatterns.Command.Scripts.Study
{
    public static class CommandProcessor
    {
        public static async Awaitable ProcessCommandsAsync(IEnumerable<ICommand> commands, CancellationToken cancellationToken)
        {
            foreach (var command in commands)
            {
                await command.ExecuteAsync(cancellationToken);
            }
        }
    }
}
