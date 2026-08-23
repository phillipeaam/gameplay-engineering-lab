namespace RPG_Tiny_Hero_Duo.Playground.Locomotion.Grounding
{
    internal readonly struct GroundedStateChange
    {
        public GroundedStateChange(bool isGrounded, bool hasChanged, bool hasLanded)
        {
            IsGrounded = isGrounded;
            HasChanged = hasChanged;
            HasLanded = hasLanded;
        }

        public bool IsGrounded { get; }
        public bool HasChanged { get; }
        public bool HasLanded { get; }
    }
}
