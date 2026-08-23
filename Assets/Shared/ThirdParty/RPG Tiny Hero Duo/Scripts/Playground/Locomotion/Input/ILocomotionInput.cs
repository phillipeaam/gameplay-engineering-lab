using UnityEngine.InputSystem;

namespace RPG_Tiny_Hero_Duo.Playground.Locomotion.Input
{
    public interface ILocomotionInput
    {
        InputAction Move { get; }
        InputAction Look { get; }
        InputAction Jump { get; }
    }
}
