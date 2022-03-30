namespace BusTicketSystem.Data.Configuration
{
    using BusTicketSystem.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    public class BusStationConfig : IEntityTypeConfiguration<BusStation>
    {
        public void Configure(EntityTypeBuilder<BusStation> builder)
        {
            builder.HasKey(bs => bs.Id);

            builder.Property(bs => bs.Name)
                .IsRequired(true)
                .IsUnicode(false)
                .HasMaxLength(50);

            builder.HasOne(bs => bs.Town)
                .WithMany(t => t.BusStations)
                .HasForeignKey(bs => bs.TownId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
