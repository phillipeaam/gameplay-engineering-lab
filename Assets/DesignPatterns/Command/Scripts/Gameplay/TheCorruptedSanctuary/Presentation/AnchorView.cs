using UnityEngine;

namespace DesignPatterns.Command.Scripts.Gameplay.TheCorruptedSanctuary.Presentation
{
    public sealed class AnchorView : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Color _dormantColor = new(0.55f, 0.1f, 0.75f);
        [SerializeField] private Color _awakenedColor = new(0.1f, 1f, 1f);
        [SerializeField] private Color _destroyedColor = new(0.08f, 0.08f, 0.08f);

        private MaterialPropertyBlock _propertyBlock;

        private void Awake()
        {
            _renderer ??= GetComponentInChildren<Renderer>();
            _propertyBlock = new MaterialPropertyBlock();
        }

        public void Present(bool awakened, bool destroyed)
        {
            var color = destroyed ? _destroyedColor : awakened ? _awakenedColor : _dormantColor;
            transform.localScale = destroyed ? new Vector3(1f, 0.2f, 1f) : Vector3.one;

            if (_renderer == null)
            {
                return;
            }

            _renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor("_BaseColor", color);
            _propertyBlock.SetColor("_Color", color);
            _renderer.SetPropertyBlock(_propertyBlock);
        }
    }
}
