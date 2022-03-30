namespace P01_BillsPaymentSystem.Data
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using P01_BillsPaymentSystem.Data.EntityConfig;
    using P01_BillsPaymentSystem.Data.Models;

    public class BillsPaymentSystemContext : DbContext
    {
        public BillsPaymentSystemContext()
        { }
        public BillsPaymentSystemContext(DbContextOptions options)
            :base(options) { }
        public DbSet<User> Users { get; set; }  
        public DbSet<CreditCard> CreditCards { get; set; }      
        public DbSet<BankAccount> BankAccounts { get; set; }  
        public DbSet<PaymentMethod> PaymentMethods { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BankAccountConfiguration());
            modelBuilder.ApplyConfiguration(new CreditCardConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentMethodConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
