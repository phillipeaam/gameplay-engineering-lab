using UnityEngine;

namespace Shared.RPG_Tiny_Hero_Duo.Scripts.Locomotion
{
    public interface IMovementAnimator
    {
        void ApplyMovement(Vector2 movement);
        void SetGrounded(bool isGrounded);
    }
}
