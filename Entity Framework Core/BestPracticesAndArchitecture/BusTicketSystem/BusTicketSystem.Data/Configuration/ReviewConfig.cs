namespace BusTicketSystem.Data.Configuration
{
    using BusTicketSystem.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    public class ReviewConfig : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Content)
                .IsRequired(true)
                .IsUnicode(true)
                .HasMaxLength(255);

            builder.Property(r => r.PublishDate)
                .HasDefaultValueSql("GETDATE()");

            builder.HasOne(r => r.BusCompany)
                .WithMany(bs => bs.Reviews)
                .HasForeignKey(r => r.BusCompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Customer)
                .WithMany(c => c.Reviews)
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
