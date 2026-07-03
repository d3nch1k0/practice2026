using System;
using Xunit;
using task11;

namespace task11tests
{
    public class CalculatorTests
    {

        private const string row = @"
        public class Calculator
        {
            public int Add(int a, int b) => a + b;
            public int Minus(int a, int b) => a - b;
            public int Mul(int a, int b) => a * b;
            public int Div(int a, int b) => a / b;
        }";

        [Fact]
        public void CreateCalculator_ShouldCompileAndExecuteAllMethods_WithoutReflection()
        {

            ICalculator calculator = ClassGenerator.CreateCalculator(row);

            Assert.NotNull(calculator);


            Assert.Equal(15, calculator.Add(10, 5));
            Assert.Equal(5, calculator.Minus(10, 5));
            Assert.Equal(50, calculator.Mul(10, 5));
            Assert.Equal(2, calculator.Div(10, 5));
        }

        [Fact]
        public void CreateCalculator_InvalidCode_ShouldThrowInvalidOperationException()
        {

            string badrow = @"
            public Calculator {
                public int Add(int a, int b) => a + b;
            }";

            Assert.Throws<InvalidOperationException>(() => ClassGenerator.CreateCalculator(badrow));
        }
    }
}
