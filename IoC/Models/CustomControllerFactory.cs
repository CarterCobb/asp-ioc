using System;
using System.Collections.Generic;
using IoC.Controllers;
using IoC.Models.DependancyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;

namespace IoC.Models
{

    public class DefaultLogger<T> : ILogger<T>
    {
        private readonly string CategoryName;
        private readonly string _logPrefix;

        public DefaultLogger()
        {
            CategoryName = "Custom Logger";
            _logPrefix = "Logger:";
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new NoopDisposable();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            string message = _logPrefix;
            if (formatter != null) message += formatter(state, exception);
            Console.WriteLine($"{logLevel} - {eventId.Id} - {CategoryName} - {message}");
        }

        private class NoopDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }

    public class CustomControllerFactory : IControllerFactory
    {
        ServiceContainer container = new ServiceContainer();

        public CustomControllerFactory()
        {
            // Register Basic dependancies
            container.Register<ILogger<HomeController>, DefaultLogger<HomeController>>(ELifecycleType.TRANSIENT);

            // Register Controllers
            container.Register<Controller, HomeController>(ELifecycleType.TRANSIENT);
            container.Register<Controller, HomeController>(ELifecycleType.TRANSIENT);

        }

        /// <summary>
        /// Creates a controller if its registered in the continer.
        /// </summary>
        /// <param name="context">Controller Context</param>
        /// <returns>Controller instance</returns>
        public object CreateController(ControllerContext context)
        {
            // Get name of the controller from the RouteData
            var controller_name = context.RouteData.Values["controller"].ToString();

            // Get all the controllers implemeting the `Controller` interface
            var controllers = container.Resolve<Controller>();

            // Loop the found registered controllers and return the first one matching the name
            if (controllers is List<Controller>)
                foreach (var controller in (List<Controller>)controllers)
                    if (controller.GetType().Name.Contains(controller_name)) return controller;

            // Only one registered controller was found, return that
            return controllers;
        }

        public void ReleaseController(ControllerContext context, object controller)
        {
           // Disregard
        }
    }
}
