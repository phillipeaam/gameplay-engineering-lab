using System.Collections;
using System.Collections.Generic;
using System.Threading;
using DesignPatterns.Command.Scripts.Study;
using DesignPatterns.Command.Tests.PlayMode.TestDoubles;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace DesignPatterns.Command.Tests.PlayMode
    
{
    public class CommandsPlayModeTests
    {
        private GameActor _actor;
        private GameObject _cubeObject;
        private CancellationTokenSource _cancellationTokenSource;

        [SetUp]
        public void SetUp()
        {
            _cubeObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _cubeObject.name = "Cube";
            _actor = _cubeObject.AddComponent<GameActor>();
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            CancelAndDisposeMovement();

            if (_cubeObject is not null)
            {
                Object.Destroy(_cubeObject);
                yield return null;
            }
        }
        
        private void CancelAndDisposeMovement()
        {
            if (_cancellationTokenSource is null)
            {
                return;
            }

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
        
        [UnityTest]
        public IEnumerator ProcessCommandsAsync_CompletesWithActorAtFinalDestination()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            var cancellationToken = _cancellationTokenSource.Token;
            
            var commands = new []
            {
                new MoveCommand(_actor, Vector2.right),
                new MoveCommand(_actor, Vector2.left),
                new MoveCommand(_actor, Vector2.right)
            };
            
            var execution = CommandProcessor.ProcessCommandsAsync(commands, cancellationToken);
            
            while (!execution.IsCompleted)
            {
                yield return null;
            }
            
            execution.GetAwaiter().GetResult();

            Assert.That(_actor.GetPosition(), Is.EqualTo(Vector2.right));
        }

        [UnityTest]
        public IEnumerator ProcessCommandsAsync_ExecutesCommandsSequentially()
        {
            var events = new List<string>();

            var commands = new ICommand[]
            {
                new RecordingCommand("A", events),
                new RecordingCommand("B", events),
                new RecordingCommand("C", events)
            };
            
            var execution = CommandProcessor.ProcessCommandsAsync(commands, CancellationToken.None);

            while (!execution.IsCompleted)
            {
                yield return null;
            }

            execution.GetAwaiter().GetResult();

            Assert.That(events, Is.EqualTo(new[]
            {
                "A:start",
                "A:finish",
                "B:start",
                "B:finish",
                "C:start",
                "C:finish"
            }));
        }
    }
}
