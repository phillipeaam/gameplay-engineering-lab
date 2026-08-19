using System;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Extensions;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion.Movement
{
    internal static class MovementSettingsExtensions
    {
        public static IMovementSettings RequireValid(this IMovementSettings movementSettings)
        {
            movementSettings.GetValidatedMovementSpeed();
            movementSettings.GetValidatedRotationSpeed();

            return movementSettings;
        }

        public static float GetValidatedMovementSpeed(this IMovementSettings movementSettings)
        {
            if (movementSettings == null)
            {
                throw new ArgumentNullException(nameof(movementSettings));
            }

            return movementSettings.MovementSpeed.RequireNonNegativeFinite(
                nameof(movementSettings.MovementSpeed),
                movementSettings.GetType().Name);
        }

        public static float GetValidatedRotationSpeed(this IMovementSettings movementSettings)
        {
            if (movementSettings == null)
            {
                throw new ArgumentNullException(nameof(movementSettings));
            }

            return movementSettings.RotationSpeed.RequireNonNegativeFinite(
                nameof(movementSettings.RotationSpeed),
                movementSettings.GetType().Name);
        }
    }
}
