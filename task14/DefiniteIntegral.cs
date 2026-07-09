using System;
using System.Threading;

namespace task14
{
    public class DefiniteIntegral
    {
        private static void SafeAdd(ref double location, double value)
        {
            double initial;
            double newValue;
            do
            {
                initial = location;
                newValue = initial + value;
            }
            while (initial != Interlocked.CompareExchange(ref location, newValue, initial));

        }
        public static double Solve(double a, double b, Func<double, double> function, double step, int threadsnumber)
        {
            double totalsum = 0.0;
            using var barrier = new Barrier(threadsnumber + 1);
            double threadWidth = (b - a) / threadsnumber;
            Thread[] threads = new Thread[threadsnumber];

            for(int i = 0; i < threadsnumber; i++)
            {
                int threadId = i;
                threads[i] = new Thread(() =>
                {
                    double start = a + threadId * threadWidth;
                    double end = start + threadWidth;

                    if (threadId == threadsnumber - 1)
                        end = b;

                    double localSum = 0.0;

                    
                    int steps = (int)Math.Round((end - start) / step);
                    if (steps == 0) steps = 1;

                    double actualStep = (end - start) / steps;

                    for (int j = 0; j < steps; j++)
                    {
                        double x1 = start + j * actualStep;
                        double x2 = start + (j + 1) * actualStep;

                        localSum += (function(x1) + function(x2)) / 2.0 * (x2 - x1);
                    }
                    SafeAdd(ref totalsum, localSum);
                    barrier.SignalAndWait();
                });

                threads[i].Start();
            }
            barrier.SignalAndWait();
            return totalsum;
        }
    }
}
