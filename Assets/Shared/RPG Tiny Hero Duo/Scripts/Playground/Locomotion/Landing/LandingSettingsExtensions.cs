using System;
using Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Extensions;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion.Landing
{
    internal static class LandingSettingsExtensions
    {
        public static ILandingSettings RequireValid(this ILandingSettings landingSettings)
        {
            landingSettings.GetValidatedLandingDuration();
            landingSettings.GetValidatedLandingMovementMultiplier();

            return landingSettings;
        }

        public static float GetValidatedLandingDuration(this ILandingSettings landingSettings)
        {
            if (landingSettings == null)
            {
                throw new ArgumentNullException(nameof(landingSettings));
            }

            return landingSettings.LandingDuration.RequireNonNegativeFinite(
                nameof(landingSettings.LandingDuration),
                landingSettings.GetType().Name);
        }

        public static float GetValidatedLandingMovementMultiplier(this ILandingSettings landingSettings)
        {
            if (landingSettings == null)
            {
                throw new ArgumentNullException(nameof(landingSettings));
            }

            return landingSettings.LandingMovementMultiplier.RequireNormalizedFinite(
                nameof(landingSettings.LandingMovementMultiplier),
                landingSettings.GetType().Name);
        }
    }
}
