using System;
using System.Collections.Generic;
using System.Text;

namespace task17
{
    public class TestCommand(int id) : ICommand
    {
        int counter = 0;

        public void Execute()
        {
            Console.WriteLine($"Поток {id} вызов {++counter}");
        }
    }
}
