using Xunit;
using IoC.Models.DependancyInjection;
using System;

namespace ServiceContainer.Tests
{
    public class ServiceContainerTests
    {
        [Fact]
        public void AddSingleton()
        {
            // Create container and register it with the calculator class
            var container = new IoC.Models.DependancyInjection.ServiceContainer();
            container.Register<ICalculator, Calculator>(ELifecycleType.SINGLETON);

            // Resolve the calculator from the container 2 times
            var calculator1 = container.Resolve<ICalculator>();
            var calculator2 = container.Resolve<ICalculator>();

            // Ensure that the calculators have an implementation of type `Calculator`
            Assert.IsType<Calculator>(calculator1);
            Assert.IsType<Calculator>(calculator2);

            // Per singleton definition make sure the ID of the calculators are the same (single instance per execution)
            Assert.Equal(((Calculator)calculator1).ID.ToString(), ((Calculator)calculator2).ID.ToString());
        }

        [Fact]
        public void AddTransient()
        {
            // Create container and register it with the calculator class
            var container = new IoC.Models.DependancyInjection.ServiceContainer();
            container.Register<ICalculator, Calculator>(ELifecycleType.TRANSIENT);

            // Resolve the calculator from the container 2 times
            var calculator1 = container.Resolve<ICalculator>();
            var calculator2 = container.Resolve<ICalculator>();

            // Ensure that the calculators have an implementation of type `Calculator`
            Assert.IsType<Calculator>(calculator1);
            Assert.IsType<Calculator>(calculator2);

            // Per transient definition make sure the ID of the calculators are different (new instnace per request)
            Assert.NotEqual(((Calculator)calculator1).ID.ToString(), ((Calculator)calculator2).ID.ToString());
        }

        [Fact]
        public void RecursiveResolution()
        {
            // Create container and register it with the calculator and view class
            var container = new IoC.Models.DependancyInjection.ServiceContainer();
            container.Register<ICalculator, Calculator>(ELifecycleType.TRANSIENT);
            container.Register<IComparable, View>(ELifecycleType.TRANSIENT);

            // Resolve View
            var view = container.Resolve<IComparable>();

            // Ensure the view is the correct type
            Assert.IsType<View>(view);

            // Get Calculator from view
            var calculator = ((View)view).GetCalculator();

            // Ensure the calculator was initialized
            Assert.Equal(2, calculator.Add(1, 1));
        }
    }
}
