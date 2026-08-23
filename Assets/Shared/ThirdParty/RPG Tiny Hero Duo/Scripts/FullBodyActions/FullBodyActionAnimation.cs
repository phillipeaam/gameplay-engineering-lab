namespace RPG_Tiny_Hero_Duo.FullBodyActions
{
    public readonly struct FullBodyActionAnimation
    {
        public FullBodyActionAnimation(FullBodyAction action, bool isActive)
        {
            Action = action;
            IsActive = isActive;
        }

        public FullBodyAction Action { get; }
        public bool IsActive { get; }
    }
}
