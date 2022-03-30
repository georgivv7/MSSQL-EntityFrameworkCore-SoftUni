namespace VaporStore.Data
{
	using Microsoft.EntityFrameworkCore;
    using VaporStore.Data.Models;

    public class VaporStoreDbContext : DbContext
	{
		public VaporStoreDbContext()
		{
		}

		public VaporStoreDbContext(DbContextOptions options)
			: base(options)
		{
		}
		public DbSet<Game> Games { get; set; }
		public DbSet<Developer> Developers { get; set; }
		public DbSet<Genre> Genres { get; set; }	
		public DbSet<Tag> Tags { get; set; }	
		public DbSet<GameTag> GameTags { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Card> Cards { get; set; }	
		public DbSet<Purchase> Purchases { get; set; }		
		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			if (!options.IsConfigured)
			{
				options
					.UseSqlServer(Configuration.ConnectionString);
			}
		}

		protected override void OnModelCreating(ModelBuilder model)
		{
			model.Entity<GameTag>(entity =>
			{
				entity.HasKey(e => new { e.GameId, e.TagId });

				entity.HasOne(e => e.Tag)
					.WithMany(t => t.GameTags)
					.HasForeignKey(e => e.TagId)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(e => e.Game)
					.WithMany(g => g.GameTags)
					.HasForeignKey(e => e.GameId)
					.OnDelete(DeleteBehavior.Restrict);
			});
		}
	}
}