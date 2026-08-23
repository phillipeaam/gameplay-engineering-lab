using System;
using UnityEngine;

namespace ModuleHosting.Scripts
{
    public sealed class ModuleHost<TModule> where TModule : class, ITickableModule
    {
        private readonly ILogger _logger;
        private readonly Action<ModuleFailure> _failureHandler;

        private TModule _module;
        private Func<TModule> _factory;

        private ModuleHostState _state = ModuleHostState.Uninitialized;

        public ModuleHost(Func<TModule> factory, Action<ModuleFailure> failureHandler, ILogger logger = null)
        {
            _factory = factory
                       ?? throw new ArgumentNullException(nameof(factory));

            _failureHandler = failureHandler
                              ?? throw new ArgumentNullException(nameof(failureHandler));

            _logger = logger
                      ?? Debug.unityLogger
                      ?? throw new InvalidOperationException("No Unity logger is available.");
        }

        public void Compose()
        {
            if (_state != ModuleHostState.Uninitialized)
            {
                return;
            }

            try
            {
                _module = _factory() ??
                          throw new InvalidOperationException(
                              $"The factory returned no {typeof(TModule).Name} instance.");

                _state = ModuleHostState.Ready;
            }
            catch (Exception exception)
            {
                HandleFailure(ModuleFailureStage.Composition, exception);
            }
            finally
            {
                _factory = null;
            }
        }

        public void Enable()
        {
            if (_state != ModuleHostState.Ready)
            {
                return;
            }

            try
            {
                _module.Enable();
            }
            catch (Exception exception)
            {
                DisableLocally();
                HandleFailure(ModuleFailureStage.Enable, exception);
            }
        }

        public void Disable()
        {
            if (_state != ModuleHostState.Ready)
            {
                return;
            }

            try
            {
                _module.Disable();
            }
            catch (Exception exception)
            {
                HandleFailure(ModuleFailureStage.Disable, exception);
            }
        }

        public void Tick(float deltaTime)
        {
            if (_state != ModuleHostState.Ready)
            {
                return;
            }

            try
            {
                _module.Tick(deltaTime);
            }
            catch (Exception exception)
            {
                DisableLocally();
                HandleFailure(ModuleFailureStage.Tick, exception);
            }
        }

        private void DisableLocally()
        {
            try
            {
                _module.Disable();
            }
            catch (Exception disableException)
            {
                LogExceptionSafely(disableException);
            }
        }

        private void HandleFailure(ModuleFailureStage stage, Exception exception)
        {
            _state = ModuleHostState.Faulted;

            var failure = new ModuleFailure(stage, exception);

            try
            {
                _failureHandler(failure);
            }
            catch (Exception handlerException)
            {
                LogExceptionSafely(handlerException);
            }
        }

        private void LogExceptionSafely(Exception exception)
        {
            try
            {
                _logger.LogException(exception);
            }
            catch
            {
                // Logging is best effort and must not escape the failure boundary.
            }
        }
    }
}
