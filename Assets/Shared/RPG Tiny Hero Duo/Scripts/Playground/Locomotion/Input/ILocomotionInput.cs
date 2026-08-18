using UnityEngine.InputSystem;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion.Input
{
    public interface ILocomotionInput
    {
        InputAction Move { get; }
        InputAction Look { get; }
        InputAction Jump { get; }
    }
}
