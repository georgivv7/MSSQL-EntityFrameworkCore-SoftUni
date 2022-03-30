namespace BusTicketSystem.Client.Commands
{
    using BusTicketSystem.Data;
    using BusTicketSystem.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    public class PublishReviewCommand
    {
        private const string InvalidCustomerMessage = "Customer was not found!";
        private const string InvalidBusCompnayMessage = "Bus company was not found!";

        //publish-review {Customer ID} {Grade} {Bus Company Name} {Content}
        public static string Execute(string[] data)
        {
            int customerId = int.Parse(data[0]);
            double grade = double.Parse(data[1]);
            string busCompanyName = data[2];
            string content = data[3];

            using (var context = new BusTicketSystemContext())
            {
                var customer = context.Customers
                    .SingleOrDefault(c => c.Id == customerId);

                if (customer == null)
                {
                    throw new ArgumentException(InvalidCustomerMessage);
                }

                var busCompany = context.BusCompanies
                    .SingleOrDefault(bc => bc.Name == busCompanyName);

                if (busCompany == null)
                {
                    throw new ArgumentException(InvalidBusCompnayMessage);
                }

                Review review = new Review(content, grade, busCompany, customer);
                context.Reviews.Add(review);
                context.SaveChanges();

                var fullName = $"{customer.FirstName} {customer.LastName}";

                return $"Customer {fullName} published review for company {busCompanyName}";
            }         
        }
    }
}
