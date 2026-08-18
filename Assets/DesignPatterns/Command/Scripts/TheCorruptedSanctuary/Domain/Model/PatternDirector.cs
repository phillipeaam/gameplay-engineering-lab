using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DesignPatterns.Command.Scripts.TheCorruptedSanctuary.Domain.Model
{
    public sealed class PatternDirector
    {
        private readonly List<GuardianPattern> _patterns;
        private DeterministicRandom _random;
        private string _lastPatternId;

        public PatternDirector(string seed, IEnumerable<GuardianPattern> patterns)
        {
            _patterns = patterns?.ToList() ?? throw new ArgumentNullException(nameof(patterns));
            if (_patterns.Count == 0)
            {
                throw new ArgumentException("At least one guardian pattern is required.", nameof(patterns));
            }

            Seed = string.IsNullOrWhiteSpace(seed) ? "FOREST-0000" : seed;
            _random = new DeterministicRandom(Seed);
        }

        public string Seed { get; }

        public GuardianPattern Select(int destroyedAnchorCount)
        {
            var tier = Mathf.Clamp(destroyedAnchorCount + 1, 1, 3);
            var candidates = _patterns
                .Where(pattern => pattern.Tier == tier && pattern.Id != _lastPatternId)
                .OrderBy(pattern => pattern.Id, StringComparer.Ordinal)
                .ToList();

            if (candidates.Count == 0)
            {
                candidates = _patterns
                    .Where(pattern => pattern.Tier == tier)
                    .OrderBy(pattern => pattern.Id, StringComparer.Ordinal)
                    .ToList();
            }

            if (candidates.Count == 0)
            {
                throw new InvalidOperationException($"No guardian pattern exists for tier {tier}.");
            }

            var selected = candidates[_random.Next(candidates.Count)];
            _lastPatternId = selected.Id;
            return selected;
        }
    }

    public struct DeterministicRandom
    {
        private uint _state;

        public DeterministicRandom(string seed)
        {
            _state = Hash(seed);
            if (_state == 0)
            {
                _state = 0x9E3779B9u;
            }
        }

        public int Next(int maximumExclusive)
        {
            if (maximumExclusive <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumExclusive));
            }

            var value = NextUInt();
            return (int)(value % (uint)maximumExclusive);
        }

        private uint NextUInt()
        {
            var value = _state;
            value ^= value << 13;
            value ^= value >> 17;
            value ^= value << 5;
            _state = value;
            return value;
        }

        private static uint Hash(string value)
        {
            const uint offset = 2166136261;
            const uint prime = 16777619;
            var hash = offset;

            foreach (var character in value ?? string.Empty)
            {
                hash ^= character;
                hash *= prime;
            }

            return hash;
        }
    }
}
