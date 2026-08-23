namespace RPG_Tiny_Hero_Duo.FullBodyActions
{
    public interface IFullBodyActionAnimator
    {
        void PlayOnce(FullBodyAction action);
        void PlayLoop(FullBodyAction action);
        void Reset();
    }
}
