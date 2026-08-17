using UnityEngine.InputSystem;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.FullBodyActions
{
    public interface IFullBodyActionsInput
    {
        InputAction Fall { get; }
        InputAction Stun { get; }
        InputAction HitReaction { get; }
        InputAction Death { get; }
        InputAction Loop { get; }
    }
}
