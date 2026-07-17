using System;
using System.Collections.Generic;

namespace task17
{
    public class RoundRobinScheduler : IScheduler
    {
        private readonly Queue<ICommand> _commands = new();
        private readonly object _lock = new();

        public bool HasCommand()
        {
            lock (_lock)
            {
                return _commands.Count > 0;
            }
        }

        public ICommand Select()
        {
            lock (_lock)
            {
                if (_commands.Count == 0) return null;
                
                var command = _commands.Dequeue();
                
                if (command is ILongCommand longCommand && !longCommand.IsCompleted)
                {
                    _commands.Enqueue(command);
                }
                
                return command;
            }
        }

        public void Add(ICommand cmd)
        {
            if (cmd == null) throw new ArgumentNullException(nameof(cmd));

            lock (_lock)
            {
                _commands.Enqueue(cmd);
            }
        }
    }
}