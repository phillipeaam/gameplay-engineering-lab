using UnityEngine;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion.Landing
{
    internal sealed class LandingRecovery
    {
        public float TimeRemaining { get; private set; }

        public void Tick(float deltaTime)
        {
            if (TimeRemaining > 0f)
            {
                TimeRemaining = Mathf.Max(0f, TimeRemaining - deltaTime);
            }
        }

        public void Start(float duration)
        {
            TimeRemaining = Mathf.Max(0f, duration);
        }

        public float GetMovementMultiplier(ILandingSettings settings)
        {
            if (TimeRemaining <= 0f || settings.LandingDuration <= 0f)
            {
                return 1f;
            }

            float recoveryProgress = 1f - TimeRemaining / settings.LandingDuration;

            return Mathf.Lerp(
                settings.LandingMovementMultiplier,
                1f,
                Mathf.Clamp01(recoveryProgress));
        }
    }
}
