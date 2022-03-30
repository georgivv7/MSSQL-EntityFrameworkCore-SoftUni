namespace BusTicketSystem.Data.Configuration
{
    using BusTicketSystem.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    public class TripConfig : IEntityTypeConfiguration<Trip>
    {
        public void Configure(EntityTypeBuilder<Trip> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasOne(t => t.BusCompany)
                .WithMany(bs => bs.Trips)
                .HasForeignKey(t => t.BusCompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.OriginBusStation)
                .WithMany(bs => bs.StartingTrips)
                .HasForeignKey(t => t.OriginBusStationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.DestinationBusStation)
                .WithMany(bs => bs.ArrivingTrips)
                .HasForeignKey(t => t.DestinationBusStationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
