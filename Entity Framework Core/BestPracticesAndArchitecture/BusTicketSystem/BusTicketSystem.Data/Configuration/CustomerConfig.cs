namespace BusTicketSystem.Data.Configuration
{
    using BusTicketSystem.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class CustomerConfig : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.FirstName)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(50);

            builder.Property(c => c.LastName)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(50);

            builder.HasOne(c => c.Hometown)
                .WithMany(h => h.Customers)
                .HasForeignKey(c => c.HometownId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
