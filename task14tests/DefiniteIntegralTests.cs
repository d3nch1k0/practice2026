using System; 
using Xunit;
using task14; 

namespace task14tests
{ 
    public class DefiniteIntegralTests 
    { 
        static readonly Func<double, double> X = (double x) => x; 
        static readonly Func<double, double> SIN = (double x) => Math.Sin(x);

        [Fact]
        public void Test_LinearFunction_TwoThreads() 
        {
            Assert.Equal(0, DefiniteIntegral.Solve(-1, 1, X, 1e-4, 2), 1e-4);
        } 

        [Fact] 
        public void Test_SinFunction_EightThreads() 
        {
            Assert.Equal(0, DefiniteIntegral.Solve(-1, 1, SIN, 1e-5, 8), 1e-4);
        } 

        [Fact]
        public void Test_LinearFunction_PositiveInterval() 
        {
            Assert.Equal(12.5, DefiniteIntegral.Solve(0, 5, X, 1e-6, 8), 1e-5);
        }
    } 
}
