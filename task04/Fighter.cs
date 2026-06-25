using System;
using System.Collections.Generic;
using System.Text;

namespace task04
{
    public class Fighter : ISpaceship
    {
        public int Speed => 100;
        public int FirePower => 50;

        public void MoveForward()
        {
            Console.WriteLine($"Speed: {Speed}");
        }
        public void Rotate(int angle)
        {
            Console.WriteLine($"Rotat: {angle}");
        }
        public void Fire()
        {
            Console.WriteLine($"Damage: {FirePower}");
        }
    }
}
