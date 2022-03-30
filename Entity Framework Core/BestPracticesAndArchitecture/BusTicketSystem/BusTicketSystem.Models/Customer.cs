namespace BusTicketSystem.Models
{
    using BusTicketSystem.Models.Enumerations;
    using System;
    using System.Collections.Generic;

    public class Customer
    {
        private Customer()
        {
            Tickets = new HashSet<Ticket>();
            Reviews = new HashSet<Review>();
            BankAccounts = new HashSet<BankAccount>();
        }
        public Customer(string firstName, string lastName, Town homeTown)
        {
            FirstName = firstName;
            LastName = lastName;
            Hometown = homeTown;
        }
        public int Id { get; set; } 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }

        public int HometownId { get; set; }
        public Town Hometown { get; set; }

        public ICollection<Ticket> Tickets { get; set; }    
        public ICollection<Review> Reviews { get; set; }        
        public ICollection<BankAccount> BankAccounts { get; set; }     
    }
}