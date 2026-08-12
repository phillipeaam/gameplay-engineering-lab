using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DesignPatterns.Command.Scripts.Gameplay.TheCorruptedSanctuary.Domain.Model
{
    public sealed class TurnSimulator
    {
        public const int MaximumTurns = 8;

        public CommandValidation ValidatePlan(EncounterState state, TurnPlan plan)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            if (plan == null)
            {
                throw new ArgumentNullException(nameof(plan));
            }

            var position = state.Player.Position;
            var dashUsed = false;

            foreach (var command in plan.Commands)
            {
                var validation = ValidateCommand(state.Grid, position, state.Player.DashCooldown, dashUsed, command);
                if (!validation.IsValid)
                {
                    return validation;
                }

                position = GetDestination(position, command);
                dashUsed |= command.Type == PlayerCommandType.Dash;
            }

            return CommandValidation.Valid();
        }

        public TurnResult Simulate(EncounterState initialState, TurnPlan plan, GuardianPattern pattern)
        {
            if (initialState == null)
            {
                throw new ArgumentNullException(nameof(initialState));
            }

            if (pattern == null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            var validation = ValidatePlan(initialState, plan);
            if (!validation.IsValid)
            {
                throw new InvalidOperationException(validation.Message);
            }

            var state = initialState.Clone();
            var beatResults = new List<BeatResult>(TurnPlan.CommandCount);
            var dashUsed = false;
            var anchorDestroyedThisTurn = false;
            var scoreGained = 0;

            for (var beatIndex = 0; beatIndex < TurnPlan.CommandCount; beatIndex++)
            {
                var command = plan.Commands[beatIndex];
                var intent = pattern.Intents[beatIndex];
                var startPosition = state.Player.Position;
                var guarding = command.Type == PlayerCommandType.Guard;

                state.Player.Position = GetDestination(startPosition, command);
                dashUsed |= command.Type == PlayerCommandType.Dash;
                state.AwakenAdjacentAnchors();
                var awakenedAnchorIds = state.Anchors
                    .Where(anchor => anchor.Awakened && !anchor.Destroyed)
                    .Select(anchor => anchor.Id)
                    .ToArray();

                var playerWasHit = intent.Affects(state.Player.Position);
                var guardConsumed = playerWasHit && guarding;
                if (playerWasHit && !guardConsumed)
                {
                    state.Player.Health = Mathf.Max(0, state.Player.Health - 1);
                }

                var destroyedAnchorId = DestroyFirstAffectedAnchor(state, intent);
                if (destroyedAnchorId >= 0)
                {
                    anchorDestroyedThisTurn = true;
                    var anchorScore = 1000 * state.Multiplier;
                    scoreGained += anchorScore;
                    state.Score += anchorScore;
                    state.Multiplier++;
                }

                beatResults.Add(new BeatResult(
                    beatIndex,
                    command,
                    intent,
                    startPosition,
                    state.Player.Position,
                    playerWasHit,
                    guardConsumed,
                    destroyedAnchorId,
                    awakenedAnchorIds));

                if (state.Player.Health <= 0)
                {
                    break;
                }
            }

            state.Turn++;
            state.Player.DashCooldown = dashUsed
                ? 1
                : Mathf.Max(0, state.Player.DashCooldown - 1);

            var tookDamage = beatResults.Any(beat => beat.PlayerWasHit && !beat.GuardConsumed);
            if (!tookDamage)
            {
                var flawlessScore = 250 * state.Multiplier;
                scoreGained += flawlessScore;
                state.Score += flawlessScore;
            }

            if (!anchorDestroyedThisTurn)
            {
                state.Multiplier = 1;
            }

            state.ResetAwakenedAnchors();

            var outcome = EvaluateOutcome(state);
            return new TurnResult(state, beatResults, outcome, scoreGained);
        }

        private static CommandValidation ValidateCommand(
            GridState grid,
            Vector2Int position,
            int dashCooldown,
            bool dashUsed,
            PlayerCommandData command)
        {
            return command.Type switch
            {
                PlayerCommandType.Step => grid.ValidateStep(position, command.Direction),
                PlayerCommandType.Dash when dashCooldown > 0 =>
                    CommandValidation.Invalid("Dash recovers next turn."),
                PlayerCommandType.Dash when dashUsed =>
                    CommandValidation.Invalid("Dash can be used only once per turn."),
                PlayerCommandType.Dash => grid.ValidateDash(position, command.Direction),
                PlayerCommandType.Guard => CommandValidation.Valid(),
                PlayerCommandType.Wait => CommandValidation.Valid(),
                _ => CommandValidation.Invalid("Unknown player command.")
            };
        }

        private static Vector2Int GetDestination(Vector2Int position, PlayerCommandData command)
        {
            return command.Type switch
            {
                PlayerCommandType.Step => position + command.Direction,
                PlayerCommandType.Dash => position + command.Direction * 2,
                _ => position
            };
        }

        private static int DestroyFirstAffectedAnchor(EncounterState state, GuardianIntent intent)
        {
            var anchor = state.Anchors
                .Where(candidate => !candidate.Destroyed && candidate.Awakened && intent.Affects(candidate.Position))
                .OrderBy(candidate => candidate.Id)
                .FirstOrDefault();

            if (anchor == null)
            {
                return -1;
            }

            anchor.Destroyed = true;
            anchor.Awakened = false;
            state.Grid.AddBlockedCell(anchor.Position);
            return anchor.Id;
        }

        private static EncounterOutcome EvaluateOutcome(EncounterState state)
        {
            if (state.Player.Health <= 0)
            {
                return EncounterOutcome.HealthDefeat;
            }

            if (state.AllAnchorsDestroyed)
            {
                return EncounterOutcome.Victory;
            }

            return state.Turn >= MaximumTurns
                ? EncounterOutcome.TurnLimitDefeat
                : EncounterOutcome.Continue;
        }
    }
}
