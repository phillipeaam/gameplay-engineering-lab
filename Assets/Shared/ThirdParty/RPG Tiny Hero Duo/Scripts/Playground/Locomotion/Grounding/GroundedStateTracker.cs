using UnityEngine;

namespace RPG_Tiny_Hero_Duo.Playground.Locomotion.Grounding
{
    internal sealed class GroundedStateTracker
    {
        private bool? _previousGroundedState;

        public GroundedStateChange Update(CollisionFlags collisionFlags)
        {
            bool grounded = (collisionFlags & CollisionFlags.Below) != 0;
            bool hasChanged = !_previousGroundedState.HasValue || grounded != _previousGroundedState.Value;
            bool hasLanded = grounded && _previousGroundedState == false;

            if (hasChanged)
            {
                _previousGroundedState = grounded;
            }

            return new GroundedStateChange(grounded, hasChanged, hasLanded);
        }
    }
}
