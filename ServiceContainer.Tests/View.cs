using System;
namespace ServiceContainer.Tests
{
    public class View : IComparable
    {
        public Guid ID { get; } = Guid.NewGuid();
        private ICalculator calculator;

        public View(ICalculator calc)
        {
            calculator = calc;
        }

        public ICalculator GetCalculator() => calculator;

        public int CompareTo(object? obj) => 0;
    }
}
