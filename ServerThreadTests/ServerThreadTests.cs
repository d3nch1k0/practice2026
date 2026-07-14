using System;
using Xunit;
using task17;

namespace ServerThreadTests
{
    public class ServerThreadTests : IDisposable
    {
        private class Command : ICommand
        {
            public bool Executed { get; private set; }
            public Action OnExecute { get; set; }

            public void Execute()
            {
                Executed = true;
                OnExecute?.Invoke();
            }
        }

        public ServerThreadTests()
        {
            ExceptionHandler.Clear();
        }

        public void Dispose()
        {
            ExceptionHandler.Clear();
        }

        [Fact]
        public void HardStop_ShouldStopImmediately_AndIgnoreRemainingCommands()
        {
            var server = new ServerThread();
            var c1 = new Command();
            var c2 = new Command();
            var hardStop = new HardStopCommand(server);
            var c3 = new Command();
            var c4 = new Command();

            server.Add(c1);
            server.Add(c2);
            server.Add(hardStop);
            server.Add(c3);
            server.Add(c4);

            server.Start();
            server.Join();


            Assert.True(c1.Executed);
            Assert.True(c2.Executed);
            Assert.False(c3.Executed);
            Assert.False(c4.Executed);
        }

        [Fact]
        public void SoftStop_ShouldDrainAllCommands_BeforeStopping()
        {
            var server = new ServerThread();
            var c1 = new Command();
            var c2 = new Command();
            var softStop = new SoftStopCommand(server);
            var c3 = new Command();
            var c4 = new Command();

            server.Add(c1);
            server.Add(c2);
            server.Add(softStop);
            server.Add(c3);
            server.Add(c4);


            server.Start();
            server.Join();

            Assert.True(c1.Executed);
            Assert.True(c2.Executed);
            Assert.True(c3.Executed);
            Assert.True(c4.Executed);
        }

        [Fact]
        public void HardAndSoftStop_ExecutedOnWrongThread_ShouldThrowInvalidOperationException()
        {

            var server = new ServerThread();
            var hardStop = new HardStopCommand(server);
            var softStop = new SoftStopCommand(server);

            Assert.Throws<InvalidOperationException>(() => hardStop.Execute());
            Assert.Throws<InvalidOperationException>(() => softStop.Execute());
        }

    }
}
