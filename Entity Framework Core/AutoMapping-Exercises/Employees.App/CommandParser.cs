namespace Employees.App
{
    using Employees.App.Commands.Contracts;
    using System;
    using System.Linq;
    using System.Reflection;

    internal class CommandParser
    {
        public static ICommand ParseCommand(IServiceProvider serviceProvider, string commandName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var commandTypes = assembly.GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(ICommand)));

            var commandType = commandTypes.SingleOrDefault(c => c.Name == $"{commandName}Command");

            if (commandType == null)
            {
                throw new InvalidOperationException("Invalid command.");    
            }

            var constructor = commandType.GetConstructors().FirstOrDefault();

            var constructorParams = constructor
                .GetParameters()
                .Select(pi => pi.ParameterType)
                .ToArray();

            var constructorArgs = constructorParams
                .Select(serviceProvider.GetService)
                .ToArray();

            var command = (ICommand) constructor.Invoke(constructorArgs);

            return command;
        }
    }
}
