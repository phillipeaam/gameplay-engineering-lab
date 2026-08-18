using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace DesignPatterns.Command.Scripts.TheCorruptedSanctuary.Application.Execution
{
    public sealed class GameplayCommandQueue
    {
        private readonly IReadOnlyList<IGameplayCommand> _commands;

        public GameplayCommandQueue(IReadOnlyList<IGameplayCommand> commands)
        {
            _commands = commands ?? throw new ArgumentNullException(nameof(commands));
        }

        public async Awaitable ExecuteAsync(CancellationToken cancellationToken)
        {
            foreach (var command in _commands)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await command.ExecuteAsync(cancellationToken);
            }
        }
    }
}
