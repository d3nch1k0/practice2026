using System;
using System.Threading;
using Task17;


namespace task17
{
    public class SoftStopCommand : ICommand
    {
        private readonly ServerThread _serverThread;
        public SoftStopCommand(ServerThread serverThread)
        {
            _serverThread = serverThread;
        }

        public void Execute()
        {
            if (Thread.CurrentThread != _serverThread.Thread)
            {
                throw new InvalidOperationException("Эта команда не может быть выполнена в текущем потоке");
            }
            _serverThread.SoftStop();
        }
    }
}
