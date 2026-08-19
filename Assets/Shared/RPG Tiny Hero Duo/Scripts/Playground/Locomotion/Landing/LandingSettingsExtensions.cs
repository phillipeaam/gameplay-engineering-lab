using System;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Extensions;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion.Landing
{
    internal static class LandingSettingsExtensions
    {
        public static ILandingSettings RequireValid(this ILandingSettings landingSettings)
        {
            if (landingSettings == null)
            {
                throw new ArgumentNullException(nameof(landingSettings));
            }

            var ownerTypeName = landingSettings.GetType().Name;

            landingSettings.LandingDuration.RequireNonNegativeFinite(
                nameof(landingSettings.LandingDuration),
                ownerTypeName);

            landingSettings.LandingMovementMultiplier.RequireFiniteRange(
                0f,
                1f,
                nameof(landingSettings.LandingMovementMultiplier),
                ownerTypeName);

            return landingSettings;
        }
    }
}
