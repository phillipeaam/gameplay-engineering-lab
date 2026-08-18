using System;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPatterns.Command.Scripts.TheCorruptedSanctuary.Domain.Model
{
    public enum PlayerCommandType
    {
        Step,
        Dash,
        Guard,
        Wait
    }

    public enum GuardianAttackType
    {
        Wait,
        LineBlast,
        ArcSweep,
        Rupture
    }

    public enum EncounterOutcome
    {
        Continue,
        Victory,
        HealthDefeat,
        TurnLimitDefeat
    }

    public enum EncounterPhase
    {
        Intro,
        Reveal,
        Planning,
        Executing,
        Evaluating,
        Finale,
        Victory,
        Defeat,
        Results
    }

    [Serializable]
    public readonly struct PlayerCommandData : IEquatable<PlayerCommandData>
    {
        public PlayerCommandData(PlayerCommandType type, Vector2Int direction)
        {
            Type = type;
            Direction = direction;
        }

        public PlayerCommandType Type { get; }
        public Vector2Int Direction { get; }

        public static PlayerCommandData Step(Vector2Int direction) =>
            new(PlayerCommandType.Step, direction);

        public static PlayerCommandData Dash(Vector2Int direction) =>
            new(PlayerCommandType.Dash, direction);

        public static PlayerCommandData Guard() =>
            new(PlayerCommandType.Guard, Vector2Int.zero);

        public static PlayerCommandData Wait() =>
            new(PlayerCommandType.Wait, Vector2Int.zero);

        public bool Equals(PlayerCommandData other) =>
            Type == other.Type && Direction == other.Direction;

        public override bool Equals(object obj) =>
            obj is PlayerCommandData other && Equals(other);

        public override int GetHashCode() => HashCode.Combine((int)Type, Direction);

        public override string ToString() =>
            Type is PlayerCommandType.Step or PlayerCommandType.Dash
                ? $"{Type} {Direction}"
                : Type.ToString();
    }

    [Serializable]
    public sealed class GuardianIntent
    {
        private readonly HashSet<Vector2Int> _affectedCells;

        public GuardianIntent(
            GuardianAttackType type,
            IEnumerable<Vector2Int> affectedCells,
            string displayName = null)
        {
            Type = type;
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? type.ToString() : displayName;
            _affectedCells = affectedCells == null
                ? new HashSet<Vector2Int>()
                : new HashSet<Vector2Int>(affectedCells);
        }

        public GuardianAttackType Type { get; }
        public string DisplayName { get; }
        public IReadOnlyCollection<Vector2Int> AffectedCells => _affectedCells;

        public bool Affects(Vector2Int cell) => _affectedCells.Contains(cell);

        public static GuardianIntent Wait() =>
            new(GuardianAttackType.Wait, Array.Empty<Vector2Int>(), "Wait");
    }

    public sealed class GuardianPattern
    {
        public const int BeatCount = 3;

        private readonly GuardianIntent[] _intents;

        public GuardianPattern(string id, int tier, IReadOnlyList<GuardianIntent> intents)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Pattern ID is required.", nameof(id));
            }

            if (intents == null || intents.Count != BeatCount)
            {
                throw new ArgumentException($"A pattern must contain exactly {BeatCount} intents.", nameof(intents));
            }

            Id = id;
            Tier = Mathf.Clamp(tier, 1, 3);
            _intents = new GuardianIntent[BeatCount];

            for (var index = 0; index < BeatCount; index++)
            {
                _intents[index] = intents[index] ?? throw new ArgumentException("Pattern intents cannot be null.");
            }
        }

        public string Id { get; }
        public int Tier { get; }
        public IReadOnlyList<GuardianIntent> Intents => _intents;
    }

    public sealed class TurnPlan
    {
        public const int CommandCount = 3;

        private readonly PlayerCommandData[] _commands;

        public TurnPlan(IReadOnlyList<PlayerCommandData> commands)
        {
            if (commands == null || commands.Count != CommandCount)
            {
                throw new ArgumentException($"A turn plan must contain exactly {CommandCount} commands.", nameof(commands));
            }

            _commands = new PlayerCommandData[CommandCount];
            for (var index = 0; index < CommandCount; index++)
            {
                _commands[index] = commands[index];
            }
        }

        public IReadOnlyList<PlayerCommandData> Commands => _commands;
    }

    public readonly struct CommandValidation
    {
        private CommandValidation(bool isValid, string message)
        {
            IsValid = isValid;
            Message = message;
        }

        public bool IsValid { get; }
        public string Message { get; }

        public static CommandValidation Valid() => new(true, string.Empty);
        public static CommandValidation Invalid(string message) => new(false, message);
    }

    public sealed class BeatResult
    {
        public BeatResult(
            int beatIndex,
            PlayerCommandData playerCommand,
            GuardianIntent guardianIntent,
            Vector2Int startPosition,
            Vector2Int endPosition,
            bool playerWasHit,
            bool guardConsumed,
            int destroyedAnchorId,
            IReadOnlyList<int> awakenedAnchorIds)
        {
            BeatIndex = beatIndex;
            PlayerCommand = playerCommand;
            GuardianIntent = guardianIntent;
            StartPosition = startPosition;
            EndPosition = endPosition;
            PlayerWasHit = playerWasHit;
            GuardConsumed = guardConsumed;
            DestroyedAnchorId = destroyedAnchorId;
            AwakenedAnchorIds = awakenedAnchorIds ?? Array.Empty<int>();
        }

        public int BeatIndex { get; }
        public PlayerCommandData PlayerCommand { get; }
        public GuardianIntent GuardianIntent { get; }
        public Vector2Int StartPosition { get; }
        public Vector2Int EndPosition { get; }
        public bool PlayerWasHit { get; }
        public bool GuardConsumed { get; }
        public int DestroyedAnchorId { get; }
        public IReadOnlyList<int> AwakenedAnchorIds { get; }
        public bool AnchorWasDestroyed => DestroyedAnchorId >= 0;
    }

    public sealed class TurnResult
    {
        public TurnResult(
            EncounterState state,
            IReadOnlyList<BeatResult> beats,
            EncounterOutcome outcome,
            int scoreGained)
        {
            State = state;
            Beats = beats;
            Outcome = outcome;
            ScoreGained = scoreGained;
        }

        public EncounterState State { get; }
        public IReadOnlyList<BeatResult> Beats { get; }
        public EncounterOutcome Outcome { get; }
        public int ScoreGained { get; }
    }
}
