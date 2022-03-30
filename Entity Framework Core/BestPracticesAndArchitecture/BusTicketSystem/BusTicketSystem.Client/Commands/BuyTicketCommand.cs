namespace BusTicketSystem.Client.Commands
{
    using BusTicketSystem.Data;
    using BusTicketSystem.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    public class BuyTicketCommand
    {
        private const string InvalidCustomerMessage = "Customer was not found!";
        private const string InvalidTripMessage = "Trip was not found!";
        private const string InsufficientFundsMessage = "Insufficient funds to buy the ticket!";
        public static string Execute(string[] data)
        {
            int customerId = int.Parse(data[0]);
            int tripId = int.Parse(data[1]);
            decimal price = decimal.Parse(data[2]);
            string seat = data[3];

            using (var context = new BusTicketSystemContext())
            {
                var customer = context.Customers
                    .SingleOrDefault(c => c.Id == customerId);

                if (customer == null)
                {
                    throw new ArgumentException(InvalidCustomerMessage);
                }

                var trip = context.Trips
                    .SingleOrDefault(t => t.Id == tripId);

                if (trip == null)
                {
                    throw new ArgumentException(InvalidTripMessage);
                }

                decimal balance = customer
                    .BankAccounts
                    .Select(c => c.Balance)
                    .FirstOrDefault();

                if (price > balance)
                {
                    throw new InvalidOperationException(InsufficientFundsMessage);
                }

                var ticket = new Ticket(price, seat, customer, trip);
                customer.Tickets.Add(ticket);

                var fullName = $"{customer.FirstName} {customer.LastName}";

                return $"Customer {fullName} bought ticket for trip {ticket.Trip.Id} for {ticket.Price}" +
                    $" on seat {ticket.Seat}";
            } 
        }
    }
}
