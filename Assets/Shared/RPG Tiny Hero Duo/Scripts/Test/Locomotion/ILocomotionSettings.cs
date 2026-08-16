namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Test.Locomotion
{
    public interface ILocomotionSettings
    {
        float MovementSpeed { get; }
        float RotationSpeed { get; }
        float JumpHeight { get; }
        bool CanDoubleJump { get; }
    }
}
