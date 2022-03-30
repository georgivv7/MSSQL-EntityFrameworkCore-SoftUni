using Microsoft.EntityFrameworkCore;
using Stations.Models;

namespace Stations.Data
{
	public class StationsDbContext : DbContext
	{
		public StationsDbContext()
		{
		}

		public StationsDbContext(DbContextOptions options)
			: base(options)
		{
		}
		public DbSet<Station> Stations { get; set; }
		public DbSet<Ticket> Tickets { get; set; }
		public DbSet<Trip> Trips { get; set; }
		public DbSet<Train> Trains { get; set; }
		public DbSet<TrainSeat> TrainSeats { get; set; }
		public DbSet<SeatingClass> SeatingClasses { get; set; }	
		public DbSet<CustomerCard> CustomerCards { get; set; }		
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder.UseSqlServer(Configuration.ConnectionString);
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<SeatingClass>(entity => entity
							 .HasAlternateKey(sc => sc.Name));

			modelBuilder.Entity<Station>(entity => entity
							 .HasAlternateKey(s => s.Name));

			modelBuilder.Entity<Train>(entity => entity
							 .HasAlternateKey(t => t.TrainNumber));

			modelBuilder.Entity<Station>(entity =>
				entity.HasMany(s => s.TripsFrom)
				.WithOne(t => t.OriginStation)
				.HasForeignKey(t => t.OriginStationId)
				.OnDelete(DeleteBehavior.Restrict));

			modelBuilder.Entity<Station>(entity =>
				entity.HasMany(s => s.TripsTo)
				.WithOne(t => t.DestinationStation)
				.HasForeignKey(t => t.DestinationStationId)
				.OnDelete(DeleteBehavior.Restrict));
		}
	}
}