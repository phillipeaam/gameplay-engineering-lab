namespace Shared.ModuleHosting
{
    public interface ITickableModule : IModule
    {
        void Tick(float deltaTime);
    }
}
