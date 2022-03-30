namespace Employees.App
{
    using System;
    using Employees.Data;
    using Employees.Services;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    public class StartUp
    {
        public static void Main()
        {
            var serviceProvider = ConfigureServices();
            Engine engine = new Engine(serviceProvider);
            engine.Run();
        }

        public static IServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddDbContext<EmployeesDbContext>(options => options
                .UseSqlServer(Configuration.ConnectionString));

            serviceCollection.AddTransient<EmployeeService>();

            serviceCollection.AddSingleton<CommandParser>();

            serviceCollection.AddAutoMapper(cfg => cfg.AddProfile<AutomapperProfile>());

            var serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }
    }
}
