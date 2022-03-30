namespace BusTicketSystem.Client.Commands
{
    using BusTicketSystem.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    public class PrintReviewsCommand
    {
        private const string InvalidBusCompnayMessage = "Bus company was not found!";
        public static string Execute(int busCompanyId)
        {
            using (var context = new BusTicketSystemContext())
            {
                var busCompany = context.BusCompanies
                    .Select(c => new
                    {
                        c.Id,
                        Reviews = c.Reviews.Select(r=> string.Concat($"{r.Id} {r.Grade} {r.PublishDate}{Environment.NewLine}",
                                    $"{r.Customer.FirstName} {r.Customer.LastName}{Environment.NewLine}" +
                                    r.Content))
                    })
                    .SingleOrDefault(bc => bc.Id == busCompanyId);

                if (busCompany == null)
                {
                    throw new ArgumentException(InvalidBusCompnayMessage);
                }
                          
                return busCompany.Reviews == null ? "There are no reviews for the company."
                    : string.Join(Environment.NewLine, busCompany.Reviews);
            }
        }
    }
}

