namespace P01_BillsPaymentSystem.Data.Models
{
    using System;
    public class CreditCard
    {
        public int CreditCardId { get; set; }
        public DateTime ExpirationDate { get; set; }    
        public decimal Limit { get; set; }
        public decimal LimitLeft => this.Limit - this.MoneyOwed;  
        public decimal MoneyOwed { get; set; }

        public int PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        public void Withdraw(decimal amount)
        {
            if (amount > this.LimitLeft)
            {
                Console.WriteLine("Insufficient funds!");
            }
            else
            {
                this.MoneyOwed += amount;
            }
        }
        public void Deposit(decimal amount)
        {
            if (amount > this.MoneyOwed)
            {
                Console.WriteLine("Deposit bigger than the owed money!");
            }
            else
            {
                this.MoneyOwed -= amount;
            }
        }
    }
}