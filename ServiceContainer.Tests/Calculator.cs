using System;
namespace ServiceContainer.Tests
{
    public class Calculator : ICalculator
    {

        public Guid ID { get; } = Guid.NewGuid();

        public Calculator()
        {
        }

        public double Add(double a, double b)
        {
            return a + b;
        }

        public double Divide(double a, double b)
        {
            return a / b;
        }

        public double Multiply(double a, double b)
        {
            return a * b;
        }

        public double Subtract(double a, double b)
        {
            return a - b;
        }
    }
}
