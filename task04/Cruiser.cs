using System;
using System.Collections.Generic;
using System.Text;

namespace task04
{
    public class Cruiser : ISpaceship
    {
        public int Speed => 50;
        public int FirePower => 100;

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
