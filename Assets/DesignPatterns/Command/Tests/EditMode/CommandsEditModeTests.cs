using System.Threading;
using DesignPatterns.Command.Scripts.Study;
using DesignPatterns.Command.Tests.EditMode.TestDoubles;
using NUnit.Framework;
using UnityEngine;

namespace DesignPatterns.Command.Tests.EditMode
    
{
    public class CommandsEditModeTests
    {
        [Test]
        public void Execute_MovesActorToTargetPosition()
        {
            var actor = new FakeActor();
            
            var command = new MoveCommand(actor, Vector2.right);
            _ = command.ExecuteAsync(CancellationToken.None);

            Assert.That(actor.GetPosition(), Is.EqualTo(Vector2.right));
        }
    }
}
