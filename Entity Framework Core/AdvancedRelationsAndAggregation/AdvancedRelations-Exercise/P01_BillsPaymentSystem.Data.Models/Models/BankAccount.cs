namespace P01_BillsPaymentSystem.Data.Models
{
    using System;
    public class BankAccount
    {
        public int BankAccountId { get; set; }
        public decimal Balance { get; set; }    
        public string BankName { get; set; }    
        public string SWIFTCode { get; set; }

        public int PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        public void Withdraw(decimal amount)
        {
            if (amount > this.Balance)
            {
                Console.WriteLine("Insufficient funds!");
            }
            else
            {
                this.Balance -= amount;
            }
            
        }
        public void Deposit(decimal amount)
        {
            this.Balance += amount;   
        }
    }
}