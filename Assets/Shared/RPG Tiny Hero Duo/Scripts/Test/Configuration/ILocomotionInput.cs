using UnityEngine.InputSystem;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Test.Configuration
{
    public interface ILocomotionInput
    {
        InputAction Move { get; }
        InputAction Look { get; }
        InputAction Jump { get; }
    }
}
