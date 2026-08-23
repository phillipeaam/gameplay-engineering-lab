namespace ModuleHosting.Scripts
{
    public interface ITickableModule : IModule
    {
        void Tick(float deltaTime);
    }
}
