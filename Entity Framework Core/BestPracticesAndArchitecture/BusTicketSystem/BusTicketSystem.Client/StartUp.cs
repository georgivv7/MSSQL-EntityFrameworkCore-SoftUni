namespace BusTicketSystem.Client
{
    using BusTicketSystem.Data;
    using BusTicketSystem.Models;
    using Core;
    using System;
    using System.Linq;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            //ResetDatabase();
            //Seed();

            CommandDispatcher commandDispatcher = new CommandDispatcher();
            Engine engine = new Engine(commandDispatcher);
            engine.Run();
        }

        private static void Seed()
        {
            using (var context = new BusTicketSystemContext())
            {
                var countries = new Country[]
                {
                   new Country("Bulgaria"),
                   new Country("Greece"),
                   new Country("Romania")
                };

                var towns = new Town[]
                {                  
                    new Town("Sofia",countries[0]),
                    new Town("Plovdiv",countries[0]),
                    new Town("Varna",countries[0]),
                    new Town("Athens",countries[1]),
                    new Town("Bucharest",countries[2])
                };

                var customers = new Customer[]
                {
                    new Customer("Ivan","Draganov",towns[0]),
                    new Customer("Georgi","Petrov",towns[1]),
                    new Customer("Sveti","Petur",towns[2])
                };

                var busCompanies = new BusCompany[]
                {
                    new BusCompany("Doris",countries[1]),
                    new BusCompany("Ivkoni",countries[0]),
                    new BusCompany("Helikopter",countries[2]),
                };

                var busStations = new BusStation[]
                {
                    new BusStation("Station Sofia", towns[0]),
                    new BusStation("Station Plovdiv", towns[1]),
                    new BusStation("Station Varna", towns[2]),
                    new BusStation("Station Athens", towns[3]),
                    new BusStation("Station Bucharest", towns[4])   
                };

                var trips = new Trip[]
                {
                    new Trip(DateTime.Parse("5/1/2019 8:30:52 AM",System.Globalization.CultureInfo.InvariantCulture),
                             DateTime.Parse("5/1/2019 10:30:52 AM",System.Globalization.CultureInfo.InvariantCulture),
                             busStations[0],busStations[1],busCompanies[1]),
                    new Trip(DateTime.Parse("6/1/2019 9:33:42 AM",System.Globalization.CultureInfo.InvariantCulture),
                             DateTime.Parse("7/1/2019 12:02:23 PM",System.Globalization.CultureInfo.InvariantCulture),
                             busStations[3],busStations[2],busCompanies[0]),
                    new Trip(DateTime.Parse("8/1/2019 18:30:52 PM",System.Globalization.CultureInfo.InvariantCulture),
                             DateTime.Parse("9/1/2019 01:49:59 AM",System.Globalization.CultureInfo.InvariantCulture),
                             busStations[1],busStations[4],busCompanies[2])
                };

                var tickets = new Ticket[]
                {
                    new Ticket(15m,"A1",customers[0],trips[0]),
                    new Ticket(175m,"B12",customers[1],trips[1]),
                    new Ticket(115m,"K7",customers[2],trips[2])
                };

                var reviews = new Review[]
                {
                    new Review("Great trip!",8.4,busCompanies[0],customers[1]),
                    new Review("Not great, not terrible!",4.4,busCompanies[1],customers[0]),
                    new Review("Poor experience!",2.4,busCompanies[2],customers[2])
                };

                var bankAccounts = new BankAccount[]
                {
                    new BankAccount("TBI815",2200m,customers[0]),
                    new BankAccount("UBB133",900m,customers[1]),
                    new BankAccount("CCB416",22200m,customers[2])
                };

                context.Countries.AddRange(countries);
                context.Towns.AddRange(towns);
                context.Customers.AddRange(customers);
                context.BusCompanies.AddRange(busCompanies);
                context.BusStations.AddRange(busStations);
                context.Trips.AddRange(trips);
                context.Tickets.AddRange(tickets);
                context.Reviews.AddRange(reviews);
                context.BankAccounts.AddRange(bankAccounts);

                context.SaveChanges();
            }
        }

        private static void ResetDatabase()
        {
            using (var context = new BusTicketSystemContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
        }
    }
}
