using System;
using NUnit.Framework;
using UnityEngine;

namespace Shared.ModuleHosting.Tests.EditMode
{
    public sealed class ModuleHostTests
    {
        /// <summary>
        /// Verifies that a composed host forwards successful lifecycle operations to its module.
        /// </summary>
        [Test]
        public void ComposeAndTick_ForwardsLifecycleToModule()
        {
            var module = new RecordingModule();
            var host = new ModuleHost<RecordingModule>(() => module, _ => { });

            host.Compose();
            host.Enable();
            host.Tick(0.25f);
            host.Disable();

            Assert.That(module.EnableCount, Is.EqualTo(1));
            Assert.That(module.TickCount, Is.EqualTo(1));
            Assert.That(module.LastDeltaTime, Is.EqualTo(0.25f));
            Assert.That(module.DisableCount, Is.EqualTo(1));
        }

        /// <summary>
        /// Verifies that a null factory result is reported as a composition failure.
        /// </summary>
        [Test]
        public void Compose_WhenFactoryReturnsNull_ReportsCompositionFailure()
        {
            ModuleFailure? recordedFailure = null;
            var host = new ModuleHost<RecordingModule>(() => null, failure => recordedFailure = failure);

            host.Compose();
            host.Tick(0.1f);

            Assert.That(recordedFailure.HasValue, Is.True);

            if (recordedFailure != null)
            {
                Assert.That(recordedFailure.Value.Stage, Is.EqualTo(ModuleFailureStage.Composition));
                Assert.That(recordedFailure.Value.Exception, Is.TypeOf<InvalidOperationException>());
            }
            else
            {
                Assert.Fail("Expected a recorded failure, but none was found.");
            }
        }

        /// <summary>
        /// Verifies that a tick failure disables the module and reports the original exception.
        /// </summary>
        [Test]
        public void Tick_WhenModuleThrows_DisablesModuleAndReportsFailure()
        {
            var expectedException = new InvalidOperationException("Tick failed.");
            ModuleFailure? recordedFailure = null;
            var module = new RecordingModule { TickException = expectedException };
            var host = new ModuleHost<RecordingModule>(() => module, failure => recordedFailure = failure);

            host.Compose();
            host.Tick(0.1f);
            host.Tick(0.1f);

            Assert.That(module.TickCount, Is.EqualTo(1));
            Assert.That(module.DisableCount, Is.EqualTo(1));
            Assert.That(recordedFailure.HasValue, Is.True);

            if (recordedFailure != null)
            {
                Assert.That(recordedFailure.Value.Stage, Is.EqualTo(ModuleFailureStage.Tick));
                Assert.That(recordedFailure.Value.Exception, Is.SameAs(expectedException));
            }
            else
            {
                Assert.Fail("Expected a recorded failure, but none was found.");
            }
        }

        /// <summary>
        /// Verifies that an enable failure attempts local cleanup and reports the failure stage.
        /// </summary>
        [Test]
        public void Enable_WhenModuleThrows_DisablesModuleAndReportsFailure()
        {
            ModuleFailure? recordedFailure = null;
            var module = new RecordingModule { EnableException = new InvalidOperationException("Enable failed.") };
            var host = new ModuleHost<RecordingModule>(() => module, failure => recordedFailure = failure);

            host.Compose();
            host.Enable();

            Assert.That(module.DisableCount, Is.EqualTo(1));
            Assert.That(recordedFailure.HasValue, Is.True);

            if (recordedFailure != null)
            {
                Assert.That(recordedFailure.Value.Stage, Is.EqualTo(ModuleFailureStage.Enable));
                Assert.That(recordedFailure.Value.Exception, Is.TypeOf<InvalidOperationException>());
            }
            else
            {
                Assert.Fail("Expected a recorded failure, but none was found.");
            }
        }

        /// <summary>
        /// Verifies that a failure handler exception is contained by the host boundary.
        /// </summary>
        [Test]
        public void FailureHandler_WhenItThrows_DoesNotEscapeHost()
        {
            var module = new RecordingModule { TickException = new InvalidOperationException("Tick failed.") };
            var handlerCalled = false;
            var logger = new FixtureLogger();
            var host = new ModuleHost<RecordingModule>(
                () => module,
                _ =>
                {
                    handlerCalled = true;
                    throw new InvalidOperationException("Handler failed.");
                },
                new Logger(logger));

            host.Compose();

            Assert.DoesNotThrow(() => host.Tick(0.1f));
            Assert.That(handlerCalled, Is.True);
            Assert.That(module.DisableCount, Is.EqualTo(1));
            Assert.That(logger.LoggedException, Is.TypeOf<InvalidOperationException>());
            Assert.That(logger.LoggedException.Message, Is.EqualTo("Handler failed."));
        }

        private sealed class FixtureLogger : ILogHandler
        {
            public Exception LoggedException { get; private set; }

            public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
            {
            }

            public void LogException(Exception exception, UnityEngine.Object context)
            {
                LoggedException = exception;
            }
        }

        private sealed class RecordingModule : ITickableModule
        {
            public int EnableCount { get; private set; }
            public int DisableCount { get; private set; }
            public int TickCount { get; private set; }
            public float LastDeltaTime { get; private set; }
            public Exception EnableException { get; set; }
            public Exception TickException { get; set; }

            public void Enable()
            {
                EnableCount++;
                ThrowIfConfigured(EnableException);
            }

            public void Disable()
            {
                DisableCount++;
            }

            public void Tick(float deltaTime)
            {
                TickCount++;
                LastDeltaTime = deltaTime;
                ThrowIfConfigured(TickException);
            }

            private static void ThrowIfConfigured(Exception exception)
            {
                if (exception != null)
                {
                    throw exception;
                }
            }
        }
    }
}
