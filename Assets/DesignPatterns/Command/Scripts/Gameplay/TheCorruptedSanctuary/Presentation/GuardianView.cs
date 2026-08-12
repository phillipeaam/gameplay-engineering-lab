using System.Threading;
using DesignPatterns.Command.Scripts.Gameplay.TheCorruptedSanctuary.Domain.Model;
using UnityEngine;

namespace DesignPatterns.Command.Scripts.Gameplay.TheCorruptedSanctuary.Presentation
{
    public sealed class GuardianView : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _anticipationDuration = 0.35f;
        [SerializeField] private Animator _animator;
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Color _anticipationColor = new(1f, 0.1f, 0.4f);

        private Color _baseColor = Color.white;
        private MaterialPropertyBlock _propertyBlock;

        private void Awake()
        {
            _animator ??= GetComponentInChildren<Animator>();
            _renderer ??= GetComponentInChildren<Renderer>();
            _propertyBlock = new MaterialPropertyBlock();
            if (_renderer != null && _renderer.sharedMaterial != null)
            {
                _baseColor = _renderer.sharedMaterial.color;
            }
        }

        public async Awaitable ExecuteAsync(GuardianIntent intent, CancellationToken cancellationToken)
        {
            if (intent.Type == GuardianAttackType.Wait)
            {
                await Awaitable.WaitForSecondsAsync(_anticipationDuration, cancellationToken);
                return;
            }

            SetColor(_anticipationColor);
            PlayState("Attack01");

            await Awaitable.WaitForSecondsAsync(_anticipationDuration, cancellationToken);
            SetColor(_baseColor);
        }

        public void PresentBeatResult(BeatResult beat)
        {
            if (beat.AnchorWasDestroyed && _animator != null)
            {
                PlayState("Dizzy");
            }
        }

        public void PresentDefeat()
        {
            PlayState("Die");
        }

        public void PresentIntroReveal()
        {
            PlayState("Attack01");
        }

        public void PresentExposed()
        {
            PlayState("Dizzy");
        }

        private void SetColor(Color color)
        {
            if (_renderer == null)
            {
                return;
            }

            _renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor("_BaseColor", color);
            _propertyBlock.SetColor("_Color", color);
            _renderer.SetPropertyBlock(_propertyBlock);
        }

        private void PlayState(string stateName)
        {
            if (_animator == null || _animator.runtimeAnimatorController == null)
            {
                return;
            }

            var shortHash = Animator.StringToHash(stateName);
            var fullHash = Animator.StringToHash($"Base Layer.{stateName}");
            if (_animator.HasState(0, shortHash))
            {
                _animator.CrossFade(shortHash, 0.08f);
            }
            else if (_animator.HasState(0, fullHash))
            {
                _animator.CrossFade(fullHash, 0.08f);
            }
        }
    }
}
