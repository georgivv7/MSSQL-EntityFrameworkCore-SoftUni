namespace Artillery.Data
{
    using Artillery.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class ArtilleryContext : DbContext
    {
        public ArtilleryContext() { }

        public ArtilleryContext(DbContextOptions options)
            : base(options) { }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Gun> Guns { get; set; }        
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Shell> Shells { get; set; }
        public DbSet<CountryGun> CountriesGuns { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Manufacturer>(entity =>
            {
                entity.HasAlternateKey(e => e.ManufacturerName);
            });

            modelBuilder.Entity<CountryGun>(entity =>
            {
                entity.HasKey(e => new { e.CountryId, e.GunId });

                entity.HasOne(e => e.Country)
                    .WithMany(c => c.CountriesGuns)
                    .HasForeignKey(e => e.CountryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Gun)
                    .WithMany(g => g.CountriesGuns)
                    .HasForeignKey(e => e.GunId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
