using System.Threading;
using DesignPatterns.Command.Scripts.Gameplay.TheCorruptedSanctuary.Domain.Model;
using UnityEngine;

namespace DesignPatterns.Command.Scripts.Gameplay.TheCorruptedSanctuary.Presentation
{
    public sealed class GameplayActorView : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _stepDuration = 0.35f;
        [SerializeField, Min(0f)] private float _dashDuration = 0.25f;
        [SerializeField] private Animator _animator;

        private Vector3 _gridOrigin;
        private float _cellSize = 1.5f;

        private void Awake()
        {
            _animator ??= GetComponentInChildren<Animator>();
        }

        public void Configure(Vector3 gridOrigin, float cellSize, Vector2Int position)
        {
            _gridOrigin = gridOrigin;
            _cellSize = cellSize;
            transform.position = GridToWorld(position);
        }

        public async Awaitable ExecuteAsync(
            PlayerCommandData command,
            Vector2Int destination,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            switch (command.Type)
            {
                case PlayerCommandType.Step:
                    PlayState("MoveFWD_Battle_InPlace_SwordAndShield");
                    await MoveToAsync(GridToWorld(destination), _stepDuration, cancellationToken);
                    break;
                case PlayerCommandType.Dash:
                    PlayState("SprintFWD_Battle_InPlace_SwordAndShield");
                    await MoveToAsync(GridToWorld(destination), _dashDuration, cancellationToken);
                    break;
                case PlayerCommandType.Guard:
                    PlayState("Defend_SwordAndShield");
                    await Awaitable.WaitForSecondsAsync(_stepDuration, cancellationToken);
                    break;
                case PlayerCommandType.Wait:
                    await Awaitable.WaitForSecondsAsync(_stepDuration, cancellationToken);
                    break;
            }
        }

        public void PresentBeatResult(BeatResult beat)
        {
            if (beat.PlayerWasHit && !beat.GuardConsumed)
            {
                PlayState("GetHit01_SwordAndShield");
            }
        }

        public void PresentFinalStrike()
        {
            PlayState("Attack04_SwordAndShiled");
        }

        public void PresentIntroReady()
        {
            PlayState("Defend_SwordAndShield");
        }

        public Vector3 GridToWorld(Vector2Int cell) =>
            _gridOrigin + new Vector3((cell.x - 1) * _cellSize, 0f, (cell.y - 1) * _cellSize);

        private async Awaitable MoveToAsync(
            Vector3 destination,
            float duration,
            CancellationToken cancellationToken)
        {
            var start = transform.position;
            var direction = destination - start;
            if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            }

            if (duration <= 0f)
            {
                transform.position = destination;
                return;
            }

            var elapsed = 0f;
            while (elapsed < duration)
            {
                cancellationToken.ThrowIfCancellationRequested();
                elapsed += Time.deltaTime;
                transform.position = Vector3.Lerp(start, destination, Mathf.Clamp01(elapsed / duration));
                await Awaitable.NextFrameAsync(cancellationToken);
            }

            transform.position = destination;
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
