using System;
using System.Collections.Concurrent;
using System.Threading;
using task17;

namespace task17
{
    public class ServerThread
    {
        private readonly BlockingCollection<ICommand> _queue = new();
        private readonly IScheduler _scheduler;
        private readonly Thread _thread;
        private readonly CancellationTokenSource _cts = new();
        private Action _behavior;
        private volatile bool _stopImmediately = false;

        public Thread Thread => _thread;

        public ServerThread(IScheduler scheduler)
        {
            _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
            _behavior = DefaultBehavior;
            _thread = new Thread(Run);
        }

        public void Start() => _thread.Start();

        public void Add(ICommand command)
        {
            try
            {
                _queue.Add(command, _cts.Token);
            }
            catch (OperationCanceledException)
            {
            }
            catch (InvalidOperationException)
            {
            }
        }

        public void UpdateBehavior(Action newBehavior)
        {
            _behavior = newBehavior ?? throw new ArgumentNullException(nameof(newBehavior));
        }

        public void HardStop()
        {
            _stopImmediately = true;
            _cts.Cancel();
        }

        public void SoftStop()
        {
            _queue.CompleteAdding();
            _cts.Cancel();

            UpdateBehavior(() =>
            {
                if (_queue.IsCompleted && !_scheduler.HasCommand())
                {
                    HardStop();
                    return;
                }
                DefaultBehavior();
            });
        }

        private void DefaultBehavior()
        {
            bool workDone = false;

            if (_queue.TryTake(out ICommand newCommand))
            {
                ExecuteCommand(newCommand);
                workDone = true;
            }

            if (_scheduler.HasCommand())
            {
                ICommand scheduledCommand = _scheduler.Select();
                if (scheduledCommand != null)
                {
                    ExecuteCommand(scheduledCommand);
                    workDone = true;
                }
            }

            if (!workDone)
            {
                try
                {
                    ICommand blockingCommand = _queue.Take(_cts.Token);
                    ExecuteCommand(blockingCommand);
                }
                catch (OperationCanceledException)
                {
                    HardStop();
                }
                catch (InvalidOperationException)
                {
                    HardStop();
                }
            }
        }

        private void ExecuteCommand(ICommand command)
        {
            try
            {
                command.Execute();
                if (command is ILongCommand longRunning && !longRunning.IsCompleted)
                {
                    _scheduler.Add(longRunning);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(command, ex);
            }
        }

        private void Run()
        {
            while (!_stopImmediately)
            {
                _behavior();
            }
        }

        public void Join() => _thread.Join();
    }
}