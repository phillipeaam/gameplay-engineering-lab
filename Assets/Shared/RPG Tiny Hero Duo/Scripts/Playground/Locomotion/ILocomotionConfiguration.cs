namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Playground.Locomotion
{
    public interface ILocomotionConfiguration
    {
        float MovementSpeed { get; }
        float RotationSpeed { get; }
        float JumpHeight { get; }
        bool CanDoubleJump { get; }
    }
}
