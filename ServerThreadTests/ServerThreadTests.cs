using Xunit;
using System;
using System.Linq;
using System.Threading;
using task17;
using Task17;

namespace Task17.Tests
{
    public class SimpleCommand : ICommand
    {
        private readonly Action _action;
        public SimpleCommand(Action action = null) => _action = action;
        public void Execute() => _action?.Invoke();
    }

    public class LongCommand : ILongCommand
    {
        private int _remaining;
        private readonly Action _onExecute;
        public bool IsCompleted => _remaining <= 0;
        public string Name { get; }

        public LongCommand(int required, string name = "", Action onExecute = null)
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

    public class SimpleScheduler : IScheduler
    {
        private readonly System.Collections.Generic.Queue<ICommand> _queue = new();
        public void Add(ICommand cmd) { if (cmd != null) _queue.Enqueue(cmd); }
        public bool HasCommand() => _queue.Count > 0;
        public ICommand Select() => _queue.Count > 0 ? _queue.Dequeue() : null;
    }

    public class ServerThreadTests : IDisposable
    {
        private readonly SimpleScheduler _scheduler;
        private readonly ServerThread _serverThread;

        public ServerThreadTests()
        {
            _scheduler = new SimpleScheduler();
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
        public void SoftStop_ProcessesSchedulerCommandsBeforeStopping()
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
        public void Thread_DoesNotBlockOnEmptyQueue_WhenSchedulerHasCommands()
        {
            var executed = false;
            var simpleCmd = new SimpleCommand(() => executed = true);

            _serverThread.Start();

            Thread.Sleep(50);

            _scheduler.Add(simpleCmd);

            _serverThread.Add(new SimpleCommand());

            Thread.Sleep(200);
            _serverThread.HardStop();
            var stopped = _serverThread.Thread.Join(TimeSpan.FromSeconds(2));

            Assert.True(stopped, "Поток завис — возможен deadlock");
            Assert.True(executed, "Команда из планировщика не выполнилась");
        }
    }
}