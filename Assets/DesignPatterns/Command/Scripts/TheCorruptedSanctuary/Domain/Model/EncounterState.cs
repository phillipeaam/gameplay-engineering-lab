using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DesignPatterns.Command.Scripts.TheCorruptedSanctuary.Domain.Model
{
    [Serializable]
    public sealed class PlayerState
    {
        public PlayerState(Vector2Int position, int health = 3, int dashCooldown = 0)
        {
            Position = position;
            Health = health;
            DashCooldown = dashCooldown;
        }

        public Vector2Int Position { get; set; }
        public int Health { get; set; }
        public int DashCooldown { get; set; }

        public PlayerState Clone() => new(Position, Health, DashCooldown);
    }

    [Serializable]
    public sealed class AnchorState
    {
        public AnchorState(int id, Vector2Int position, bool awakened = false, bool destroyed = false)
        {
            if (id < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            Id = id;
            Position = position;
            Awakened = awakened;
            Destroyed = destroyed;
        }

        public int Id { get; }
        public Vector2Int Position { get; }
        public bool Awakened { get; set; }
        public bool Destroyed { get; set; }

        public AnchorState Clone() => new(Id, Position, Awakened, Destroyed);
    }

    [Serializable]
    public sealed class EncounterState
    {
        private readonly List<AnchorState> _anchors;

        public EncounterState(
            GridState grid,
            PlayerState player,
            IEnumerable<AnchorState> anchors,
            string seed,
            int turn = 0,
            int score = 0,
            int multiplier = 1)
        {
            Grid = grid ?? throw new ArgumentNullException(nameof(grid));
            Player = player ?? throw new ArgumentNullException(nameof(player));
            _anchors = anchors?.Select(anchor => anchor ?? throw new ArgumentException("Anchors cannot contain null."))
                .ToList() ?? throw new ArgumentNullException(nameof(anchors));

            if (!Grid.IsWalkable(Player.Position))
            {
                throw new ArgumentException("Player must start on a walkable cell.", nameof(player));
            }

            if (_anchors.Select(anchor => anchor.Id).Distinct().Count() != _anchors.Count)
            {
                throw new ArgumentException("Anchor IDs must be unique.", nameof(anchors));
            }

            Seed = seed ?? string.Empty;
            Turn = turn;
            Score = score;
            Multiplier = Mathf.Max(1, multiplier);
        }

        public GridState Grid { get; }
        public PlayerState Player { get; }
        public IReadOnlyList<AnchorState> Anchors => _anchors;
        public string Seed { get; }
        public int Turn { get; set; }
        public int Score { get; set; }
        public int Multiplier { get; set; }
        public int DestroyedAnchorCount => _anchors.Count(anchor => anchor.Destroyed);
        public bool AllAnchorsDestroyed => _anchors.Count > 0 && _anchors.All(anchor => anchor.Destroyed);

        public void AwakenAdjacentAnchors()
        {
            foreach (var anchor in _anchors)
            {
                if (anchor.Destroyed)
                {
                    continue;
                }

                var distance = Mathf.Abs(anchor.Position.x - Player.Position.x) +
                               Mathf.Abs(anchor.Position.y - Player.Position.y);
                if (distance == 1)
                {
                    anchor.Awakened = true;
                }
            }
        }

        public void ResetAwakenedAnchors()
        {
            foreach (var anchor in _anchors)
            {
                if (!anchor.Destroyed)
                {
                    anchor.Awakened = false;
                }
            }
        }

        public EncounterState Clone() => new(
            Grid.Clone(),
            Player.Clone(),
            _anchors.Select(anchor => anchor.Clone()),
            Seed,
            Turn,
            Score,
            Multiplier);
    }
}
