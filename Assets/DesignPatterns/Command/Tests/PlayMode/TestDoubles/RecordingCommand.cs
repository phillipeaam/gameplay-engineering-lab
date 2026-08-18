using System.Collections.Generic;
using System.Threading;
using DesignPatterns.Command.Scripts.InitialCommandStudy;
using UnityEngine;

namespace DesignPatterns.Command.Tests.PlayMode.TestDoubles
{
    public sealed class RecordingCommand : ICommand
    {
        private readonly string _name;
        private readonly IList<string> _events;

        public RecordingCommand(string name, IList<string> events)
        {
            _name = name;
            _events = events;
        }

        public async Awaitable ExecuteAsync(CancellationToken cancellationToken)
        {
            _events.Add($"{_name}:start");

            await Awaitable.NextFrameAsync(cancellationToken);

            _events.Add($"{_name}:finish");
        }
    }
}