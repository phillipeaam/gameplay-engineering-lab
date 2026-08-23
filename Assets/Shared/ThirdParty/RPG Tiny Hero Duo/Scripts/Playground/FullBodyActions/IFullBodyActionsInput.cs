using UnityEngine.InputSystem;

namespace RPG_Tiny_Hero_Duo.Playground.FullBodyActions
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
