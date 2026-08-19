using System;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Extensions
{
    internal static class FloatExtensions
    {
        public static bool IsFinite(this float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }

        public static float RequireNonNegativeFinite(
            this float value,
            string fieldName,
            string ownerTypeName)
        {
            if (!value.IsFinite() || value < 0f)
            {
                throw new ArgumentOutOfRangeException(
                    fieldName,
                    value,
                    $"{ownerTypeName} requires '{fieldName}' to be finite and greater than or equal to zero.");
            }

            return value;
        }

        public static float RequireFiniteRange(
            this float value,
            float minimum,
            float maximum,
            string fieldName,
            string ownerTypeName)
        {
            if (!value.IsFinite() || value < minimum || value > maximum)
            {
                throw new ArgumentOutOfRangeException(
                    fieldName,
                    value,
                    $"{ownerTypeName} requires '{fieldName}' to be finite and between {minimum} and {maximum}, inclusive.");
            }

            return value;
        }
    }
}
