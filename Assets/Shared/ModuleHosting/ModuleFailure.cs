using System;

namespace Shared.ModuleHosting
{
    public readonly struct ModuleFailure
    {
        public ModuleFailure(ModuleFailureStage stage, Exception exception)
        {
            Stage = stage;
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }

        public ModuleFailureStage Stage { get; }
        public Exception Exception { get; }
    }
}
