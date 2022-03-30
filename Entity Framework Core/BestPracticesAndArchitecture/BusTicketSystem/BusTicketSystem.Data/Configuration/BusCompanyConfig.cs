namespace BusTicketSystem.Data.Configuration
{
    using BusTicketSystem.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using System;
    using System.Collections.Generic;
    using System.Text;
    public class BusCompanyConfig : IEntityTypeConfiguration<BusCompany>
    {
        public void Configure(EntityTypeBuilder<BusCompany> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsRequired(true);

            builder.HasOne(b => b.Country)
                .WithMany(c => c.BusCompanies)
                .HasForeignKey(b => b.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
