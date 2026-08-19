using UnityEngine;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion.Landing
{
    internal sealed class LandingRecovery
    {
        private readonly ILandingSettings _settings;

        private float _duration;
        private float _initialMovementMultiplier;

        private float _timeRemaining;

        public bool IsActive => _timeRemaining > 0f;

        public LandingRecovery(ILandingSettings settings)
        {
            _settings = settings.RequireValid();
        }

        public void Tick(float deltaTime)
        {
            if (_timeRemaining > 0f)
            {
                _timeRemaining = Mathf.Max(0f, _timeRemaining - deltaTime);
            }
        }

        public void Start()
        {
            _settings.RequireValid();

            _duration = _settings.LandingDuration;
            _initialMovementMultiplier = _settings.LandingMovementMultiplier;

            _timeRemaining = _duration;
        }

        public float GetMovementMultiplier()
        {
            if (_timeRemaining <= 0f || _duration <= 0f)
            {
                return 1f;
            }

            float recoveryProgress = 1f - _timeRemaining / _duration;

            return Mathf.Lerp(
                _initialMovementMultiplier,
                1f,
                Mathf.Clamp01(recoveryProgress));
        }
    }
}
