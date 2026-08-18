using System;
using System.Collections.Generic;
using System.Threading;
using DesignPatterns.Command.Scripts.TheCorruptedSanctuary.Domain.Model;
using DesignPatterns.Command.Scripts.TheCorruptedSanctuary.Presentation;
using UnityEngine;

namespace DesignPatterns.Command.Scripts.TheCorruptedSanctuary.Application.Execution
{
    public sealed class TurnRunner : MonoBehaviour
    {
        [SerializeField] private GameplayActorView _playerView;
        [SerializeField] private GuardianView _guardianView;

        public event Action<BeatResult> BeatCompleted;

        public void Configure(GameplayActorView playerView, GuardianView guardianView)
        {
            _playerView = playerView;
            _guardianView = guardianView;
        }

        public async Awaitable ExecuteAsync(TurnResult result, CancellationToken cancellationToken)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (_playerView == null || _guardianView == null)
            {
                throw new InvalidOperationException("TurnRunner requires player and guardian views.");
            }

            var commands = new List<IGameplayCommand>(result.Beats.Count);
            foreach (var beat in result.Beats)
            {
                commands.Add(new BeatPresentationCommand(_playerView, _guardianView, beat, BeatCompleted));
            }

            await new GameplayCommandQueue(commands).ExecuteAsync(cancellationToken);
        }

        private sealed class BeatPresentationCommand : IGameplayCommand
        {
            private readonly GameplayActorView _playerView;
            private readonly GuardianView _guardianView;
            private readonly BeatResult _beat;
            private readonly Action<BeatResult> _completed;

            public BeatPresentationCommand(
                GameplayActorView playerView,
                GuardianView guardianView,
                BeatResult beat,
                Action<BeatResult> completed)
            {
                _playerView = playerView;
                _guardianView = guardianView;
                _beat = beat;
                _completed = completed;
            }

            public async Awaitable ExecuteAsync(CancellationToken cancellationToken)
            {
                var playerExecution = _playerView.ExecuteAsync(_beat.PlayerCommand, _beat.EndPosition, cancellationToken);
                var guardianExecution = _guardianView.ExecuteAsync(_beat.GuardianIntent, cancellationToken);

                await playerExecution;
                await guardianExecution;

                _playerView.PresentBeatResult(_beat);
                _guardianView.PresentBeatResult(_beat);
                _completed?.Invoke(_beat);

                await Awaitable.WaitForSecondsAsync(0.15f, cancellationToken);
            }
        }
    }
}
