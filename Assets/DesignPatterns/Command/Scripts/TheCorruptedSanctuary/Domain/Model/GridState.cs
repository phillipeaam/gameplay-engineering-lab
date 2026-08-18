using System;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPatterns.Command.Scripts.TheCorruptedSanctuary.Domain.Model
{
    [Serializable]
    public sealed class GridState
    {
        private readonly HashSet<Vector2Int> _blockedCells;

        public GridState(int width, int height, IEnumerable<Vector2Int> blockedCells = null)
        {
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Grid dimensions must be positive.");
            }

            Width = width;
            Height = height;
            _blockedCells = blockedCells == null
                ? new HashSet<Vector2Int>()
                : new HashSet<Vector2Int>(blockedCells);

            foreach (var cell in _blockedCells)
            {
                if (!Contains(cell))
                {
                    throw new ArgumentException($"Blocked cell {cell} is outside the grid.", nameof(blockedCells));
                }
            }
        }

        public int Width { get; }
        public int Height { get; }
        public IReadOnlyCollection<Vector2Int> BlockedCells => _blockedCells;

        public bool Contains(Vector2Int cell) =>
            cell.x >= 1 && cell.x <= Width && cell.y >= 1 && cell.y <= Height;

        public bool IsBlocked(Vector2Int cell) => _blockedCells.Contains(cell);

        public bool IsWalkable(Vector2Int cell) => Contains(cell) && !IsBlocked(cell);

        public void AddBlockedCell(Vector2Int cell)
        {
            if (!Contains(cell))
            {
                throw new ArgumentOutOfRangeException(nameof(cell), cell, "Cell is outside the grid.");
            }

            _blockedCells.Add(cell);
        }

        public bool IsCardinalDirection(Vector2Int direction) =>
            Mathf.Abs(direction.x) + Mathf.Abs(direction.y) == 1;

        public CommandValidation ValidateStep(Vector2Int from, Vector2Int direction)
        {
            if (!IsCardinalDirection(direction))
            {
                return CommandValidation.Invalid("Step requires one cardinal direction.");
            }

            var destination = from + direction;
            return IsWalkable(destination)
                ? CommandValidation.Valid()
                : CommandValidation.Invalid($"Cell {destination} is outside the grid or blocked.");
        }

        public CommandValidation ValidateDash(Vector2Int from, Vector2Int direction)
        {
            if (!IsCardinalDirection(direction))
            {
                return CommandValidation.Invalid("Dash requires one cardinal direction.");
            }

            var firstCell = from + direction;
            var destination = firstCell + direction;

            if (!IsWalkable(firstCell))
            {
                return CommandValidation.Invalid($"Dash crosses invalid cell {firstCell}.");
            }

            return IsWalkable(destination)
                ? CommandValidation.Valid()
                : CommandValidation.Invalid($"Dash destination {destination} is outside the grid or blocked.");
        }

        public GridState Clone() => new(Width, Height, _blockedCells);
    }
}
