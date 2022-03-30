namespace BusTicketSystem.Data.Configuration
{
    using BusTicketSystem.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using System;
    using System.Collections.Generic;
    using System.Text;
    public class BankAccountConfig : IEntityTypeConfiguration<BankAccount>
    {
        public void Configure(EntityTypeBuilder<BankAccount> builder)
        {
            builder.HasKey(ba => ba.Id);

            builder.Property(ba => ba.AccountNumber)
                .HasMaxLength(30)
                .IsUnicode()
                .IsRequired();

            builder.HasOne(ba => ba.Customer)
                .WithMany(c => c.BankAccounts)
                .HasForeignKey(ba => ba.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
