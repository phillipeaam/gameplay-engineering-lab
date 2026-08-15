using UnityEngine.InputSystem;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Test.Configuration
{
    public interface IFullBodyActionsInput
    {
        InputAction Fall { get; }
        InputAction Stun { get; }
        InputAction HitReaction { get; }
        InputAction Death { get; }
    }
}
