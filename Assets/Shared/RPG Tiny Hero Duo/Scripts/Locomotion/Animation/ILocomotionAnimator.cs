using UnityEngine;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Locomotion.Animation
{
    public interface ILocomotionAnimator
    {
        void ApplyMovement(Vector2 movement);
        void SetGrounded(bool isGrounded);
        void RequestJump();
    }
}
