namespace Shared.RPG_Tiny_Hero_Duo.Scripts.FullBodyActions
{
    public interface IFullBodyActionAnimator
    {
        void PlayOnce(FullBodyAction action);
        void PlayLoop(FullBodyAction action);
        void Reset();
    }
}
