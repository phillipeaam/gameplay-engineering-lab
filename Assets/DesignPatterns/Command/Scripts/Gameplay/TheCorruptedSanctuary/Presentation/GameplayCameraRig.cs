using Unity.Cinemachine;
using UnityEngine;

namespace DesignPatterns.Command.Scripts.Gameplay.TheCorruptedSanctuary.Presentation
{
    /// <summary>Authoritative camera composition for the encounter board.</summary>
    public sealed class GameplayCameraRig : MonoBehaviour
    {
        private CinemachineCamera _gameplayCamera;
        private Transform _target;

        public CinemachineCamera GameplayCamera => _gameplayCamera;

        public void Configure(Transform boardTarget)
        {
            _target = boardTarget;
            _gameplayCamera = CreateCamera("Gameplay Camera", 20, new Vector3(11.5f, 14.5f, -11.5f));
            // The board camera is intentionally passive. Tactical readability is more
            // important than continuous follow motion; authored shots may move separately.
            _gameplayCamera.Target = default;
        }

        private CinemachineCamera CreateCamera(string cameraName, int priority, Vector3 position)
        {
            var cameraObject = new GameObject(cameraName);
            cameraObject.transform.SetParent(transform);
            cameraObject.transform.position = position;
            cameraObject.transform.LookAt(_target != null ? _target : transform);
            var camera = cameraObject.AddComponent<CinemachineCamera>();
            camera.Priority.Value = priority;
            camera.Lens.FieldOfView = 48f;
            return camera;
        }
    }
}
