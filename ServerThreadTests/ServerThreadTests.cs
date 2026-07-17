using Xunit;
using System;
using System.Linq;
using System.Threading;
using task17;


namespace task17tests
{
    public class SimpleCommand : ICommand
    {
        private readonly Action? _action;
        public SimpleCommand(Action? action = null) => _action = action;
        public void Execute() => _action?.Invoke();
    }

    public class LongCommand : ILongCommand
    {
        private int _remaining;
        private readonly Action? _onExecute;
        public bool IsCompleted => _remaining <= 0;
        public string Name { get; }

        public LongCommand(int required, string name = "", Action? onExecute = null)
        {
            _remaining = required;
            Name = name;
            _onExecute = onExecute;
        }

        public void Execute()
        {
            if (!IsCompleted)
            {
                _remaining--;
                _onExecute?.Invoke();
            }
        }
    }

    public class ServerThreadTests : IDisposable
    {
        private readonly RoundRobinScheduler _scheduler;
        private readonly ServerThread _serverThread;

        public ServerThreadTests()
        {
            _scheduler = new RoundRobinScheduler();
            _serverThread = new ServerThread(_scheduler);
        }

        public void Dispose()
        {
            if (_serverThread.Thread.IsAlive)
            {
                _serverThread.HardStop();
                _serverThread.Join();
            }
        }

        [Fact]
        public void LongCommands_EachExecutesCorrectNumberOfTimes()
        {
            var log = new System.Collections.Generic.List<string>();
            var cmdA = new LongCommand(3, "A", () => log.Add("A"));
            var cmdB = new LongCommand(3, "B", () => log.Add("B"));
            var cmdC = new LongCommand(3, "C", () => log.Add("C"));

            _serverThread.Start();
            _serverThread.Add(cmdA);
            _serverThread.Add(cmdB);
            _serverThread.Add(cmdC);

            Thread.Sleep(500);
            _serverThread.SoftStop();
            _serverThread.Join();

            Assert.Equal(3, log.Count(x => x == "A"));
            Assert.Equal(3, log.Count(x => x == "B"));
            Assert.Equal(3, log.Count(x => x == "C"));
        }

        [Fact]
        public void SoftStop_ProcessesLongCommandsBeforeStopping()
        {
            var counter = 0;
            var longCmd = new LongCommand(5, onExecute: () => Interlocked.Increment(ref counter));

            _serverThread.Start();
            _serverThread.Add(longCmd);
            Thread.Sleep(50);

            _serverThread.SoftStop();
            var stopped = _serverThread.Thread.Join(TimeSpan.FromSeconds(3));

            Assert.True(stopped, "Поток должен остановиться за 3 секунды");
            Assert.Equal(5, counter);
            Assert.False(_serverThread.Thread.IsAlive);
        }

        [Fact]
        public void RoundRobin_CommandsExecuteWithCorrectCounts()
        {
            var counterA = 0;
            var counterB = 0;
            var counterC = 0;

            var cmdA = new LongCommand(2, "A", () => Interlocked.Increment(ref counterA));
            var cmdB = new LongCommand(2, "B", () => Interlocked.Increment(ref counterB));
            var cmdC = new LongCommand(2, "C", () => Interlocked.Increment(ref counterC));

            _serverThread.Start();
            _serverThread.Add(cmdA);
            _serverThread.Add(cmdB);
            _serverThread.Add(cmdC);

            Thread.Sleep(500);
            _serverThread.SoftStop();
            _serverThread.Join();

            Assert.Equal(2, counterA);
            Assert.Equal(2, counterB);
            Assert.Equal(2, counterC);
        }

        [Fact]
        public void SimpleCommand_ExecutesOnceAndRemoved()
        {
            var counter = 0;
            var simpleCmd = new SimpleCommand(() => Interlocked.Increment(ref counter));

            _serverThread.Start();
            _serverThread.Add(simpleCmd);

            Thread.Sleep(200);
            _serverThread.SoftStop();
            _serverThread.Join();

            Assert.Equal(1, counter);
        }

        [Fact]
        public void HardStop_StopsBeforeCompletion()
        {
            var counter = 0;
            var longCmd = new LongCommand(1000, onExecute: () =>
            {
                Interlocked.Increment(ref counter);
                Thread.Sleep(1);
            });

            _serverThread.Start();
            _serverThread.Add(longCmd);
            Thread.Sleep(50);

            _serverThread.HardStop();
            var stopped = _serverThread.Thread.Join(TimeSpan.FromSeconds(2));

            Assert.True(stopped, "Поток должен остановиться за 2 секунды");
            Assert.True(counter < 1000, $"HardStop должен остановить выполнение немедленно. Выполнено: {counter}");
            Assert.False(_serverThread.Thread.IsAlive);
        }
    }
}
