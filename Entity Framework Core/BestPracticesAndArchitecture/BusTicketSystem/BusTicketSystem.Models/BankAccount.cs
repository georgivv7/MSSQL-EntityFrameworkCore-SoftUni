namespace BusTicketSystem.Models
{
    public class BankAccount
    {
        private BankAccount() { }
        public BankAccount(string accountNumber, decimal balance, Customer customer)
        {
            AccountNumber = accountNumber;
            Balance = balance;
            Customer = customer;
        }
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }  
    }
}
