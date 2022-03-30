namespace P01_BillsPaymentSystem.Data.Models
{
    using System;
    using System.Collections.Generic;

    public class User
    {
        public User()
        {
            PaymentMethods = new List<PaymentMethod>();
        }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }    
        public string Email { get; set; }    
        public string Password { get; set; }
        public ICollection<PaymentMethod> PaymentMethods { get; set; }

        public void PayBills(int userId, decimal amount)
        {
            
        }

    }
}
