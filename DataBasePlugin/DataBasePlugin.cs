using System;
using CommandLib;

namespace DatabasePlugin
{
    [PluginLoad("AnalyticsCommand")]
    public class DatabaseCommand : ICommand
    {
        public void Execute()
        {

            Console.WriteLine("База данных загружена.");
        }
    }
}
