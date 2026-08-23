using System;
using RPG_Tiny_Hero_Duo.Playground.Extensions;

namespace RPG_Tiny_Hero_Duo.Playground.Locomotion.Jumping
{
    internal static class JumpSettingsExtensions
    {
        public static IJumpSettings RequireValid(this IJumpSettings jumpSettings)
        {
            jumpSettings.GetValidatedJumpHeight();

            return jumpSettings;
        }

        public static float GetValidatedJumpHeight(this IJumpSettings jumpSettings)
        {
            if (jumpSettings == null)
            {
                throw new ArgumentNullException(nameof(jumpSettings));
            }

            return jumpSettings.JumpHeight.RequireNonNegativeFinite(
                nameof(jumpSettings.JumpHeight),
                jumpSettings.GetType().Name);
        }
    }
}
