using CommandLib;
using System;
using System.Windows.Input;

namespace AnalyticsPlugin
{
    [PluginLoad("")]
    public class AnalyticsCommand : CommandLib.ICommand
    {
        public void Execute()
        {
            Console.WriteLine("Модуль аналитики.");
        }
    }
}
