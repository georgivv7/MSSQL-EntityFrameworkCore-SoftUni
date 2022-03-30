namespace P01_BillsPaymentSystem
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using P01_BillsPaymentSystem.Data;
    using P01_BillsPaymentSystem.Data.Models;
    using P01_BillsPaymentSystem.Data.Models.Enumeration;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            var db = new BillsPaymentSystemContext();
            //db.Database.EnsureDeleted();
            //db.Database.Migrate();
            //Seed(db);

            int userId = int.Parse(Console.ReadLine());

            var user = db.Users
                .Where(u => u.UserId == userId)
                .Select(u => new
                {
                    Name = $"{u.FirstName} {u.LastName}",
                    CreditCards = u.PaymentMethods
                        .Where(u=>u.Type == PaymentType.CreditCard)
                        .Select(u=>u.CreditCard),
                    BankAccounts = u.PaymentMethods
                        .Where(u => u.Type == PaymentType.BankAccount)
                        .Select(u => u.BankAccount)
                }).FirstOrDefault();

            if (user == null)
            {
                Console.WriteLine($"User with id {userId} not found!");
            }
            else
            {
                Console.WriteLine($"User: {user.Name}");

                var bankAccounts = user.BankAccounts.ToList();
                if (bankAccounts.Any())
                {
                    Console.WriteLine("Bank Accounts:");
                    foreach (var bankAccount in bankAccounts)
                    {
                        Console.WriteLine($"-- ID: {bankAccount.BankAccountId}" + Environment.NewLine +
                                          $"--- Balance: {bankAccount.Balance:F2}" + Environment.NewLine +
                                          $"--- Bank: {bankAccount.BankName}" + Environment.NewLine +
                                          $"--- SWIFT: {bankAccount.SWIFTCode}");
                    }
                }

                var creditCards = user.CreditCards.ToList();
                if (creditCards.Any())
                {
                    Console.WriteLine("Credit Cards:");
                    foreach (var card in creditCards)
                    {
                        Console.WriteLine($"-- ID: {card.CreditCardId}" + Environment.NewLine +
                                          $"--- Limit: {card.Limit}" + Environment.NewLine +
                                          $"--- Money Owed: {card.MoneyOwed:F2}" + Environment.NewLine +
                                          $"--- Limit Left: {card.LimitLeft:F2}" + Environment.NewLine +
                                          $"--- Expiration Date: {card.ExpirationDate.ToString("yyyy/MM", CultureInfo.InvariantCulture)}");
                    }
                }
            }           
        }

        private static void Seed(BillsPaymentSystemContext db)  
        {
            using (db)
            {
                var user = new User()
                {
                    FirstName = "Pesho",
                    LastName = "Stamatov",
                    Email = "pesho@abv.bg",
                    Password = "azsympesjo"
                };

                var creditCards = new CreditCard[]
                {
                    new CreditCard()
                    {
                        ExpirationDate = DateTime.ParseExact("20.05.2020", "dd.mm.yyyy", null),
                        Limit = 1000m,
                        MoneyOwed = 5m
                    },
                    new CreditCard()
                    {
                        ExpirationDate = DateTime.ParseExact("21.07.2021", "dd.mm.yyyy", null),
                        Limit = 10000m,
                        MoneyOwed = 4000m
                    }
                };

                var bankAccount = new BankAccount()
                {
                    Balance = 1500m,
                    BankName = "Swiss Bank",
                    SWIFTCode = "SWSSBNK"
                };

                var paymentMethods = new PaymentMethod[]
                {
                    new PaymentMethod()
                    {
                        User = user,
                        CreditCard = creditCards[0],
                        Type = PaymentType.CreditCard
                    },
                    new PaymentMethod()
                    {
                        User = user,
                        CreditCard = creditCards[1],
                        Type = PaymentType.CreditCard
                    },
                    new PaymentMethod()
                    {
                        User = user,
                        BankAccount = bankAccount,
                        Type = PaymentType.BankAccount
                    }

                };

                db.Users.Add(user);
                db.BankAccounts.Add(bankAccount);
                db.CreditCards.AddRange(creditCards);
                db.PaymentMethods.AddRange(paymentMethods);

                db.SaveChanges();
            }
        }
    }
}
