using UnityEngine;

namespace RPG_Tiny_Hero_Duo.Locomotion
{
    public interface IMovementAnimator
    {
        void ApplyMovement(Vector2 movement);
        void SetGrounded(bool isGrounded);
    }
}
