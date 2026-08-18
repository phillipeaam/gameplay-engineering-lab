namespace Shared.RPG_Tiny_Hero_Duo.Scripts.FullBodyActions
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
