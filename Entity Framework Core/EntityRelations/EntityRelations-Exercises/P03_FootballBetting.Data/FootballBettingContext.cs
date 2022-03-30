namespace P03_FootballBetting.Data
{
    using Microsoft.EntityFrameworkCore;
    using P03_FootballBetting.Data.Models;

    public class FootballBettingContext : DbContext
    {
        public FootballBettingContext()
        { }
        public FootballBettingContext(DbContextOptions options)
        :base(options){}
        public DbSet<Team> Teams { get; set; }
        public DbSet<Color> Colors { get; set; }    
        public DbSet<Town> Towns { get; set; }           
        public DbSet<Country> Countries { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<PlayerStatistic> PlayerStatistics { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Bet> Bets { get; set; }
        public DbSet<User> Users { get; set; }  

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>(team =>
            {
                team.HasKey(t => t.TeamId);

                team.Property(t => t.Name)
                    .IsRequired()
                    .IsUnicode()
                    .HasMaxLength(80);

                team.Property(t => t.LogoUrl)
                    .IsRequired()
                    .IsUnicode(false);

                team.Property(t => t.Initials)
                    .IsRequired()
                    .IsUnicode()
                    .HasColumnType("CHAR(3)");

                team.HasOne(t => t.PrimaryKitColor)
                    .WithMany(c => c.PrimaryKitTeams)
                    .HasForeignKey(t => t.PrimaryKitColorId)
                    .OnDelete(DeleteBehavior.Restrict);

                team.HasOne(t => t.SecondaryKitColor)
                    .WithMany(c => c.SecondaryKitTeams)
                    .HasForeignKey(t => t.SecondaryKitColorId)
                    .OnDelete(DeleteBehavior.Restrict);

                team.HasOne(t => t.Town)
                .WithMany(t => t.Teams)
                .HasForeignKey(t => t.TownId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Color>(color =>
            {
                color.HasKey(c => c.ColorId);

                color.Property(c => c.Name)
                    .IsRequired()
                    .IsUnicode()
                    .HasMaxLength(40);
            });

            modelBuilder.Entity<Town>(town =>
            {
                town.HasKey(t => t.TownId);

                town.Property(t => t.Name)
                    .IsRequired()
                    .IsUnicode()
                    .HasMaxLength(40);

                town.HasOne(t => t.Country)
                .WithMany(c => c.Towns)
                .HasForeignKey(t => t.CountryId)
                .OnDelete(DeleteBehavior.Restrict);
                
            });

            modelBuilder.Entity<Country>(country =>
            {
                country.HasKey(c => c.CountryId);

                country.Property(c => c.Name)
                    .IsRequired()
                    .IsUnicode()
                    .HasMaxLength(80);
            });

            modelBuilder.Entity<Player>(player =>
            {
                player.HasKey(p => p.PlayerId);

                player.Property(p => p.Name)
                    .IsRequired()
                    .IsUnicode()
                    .HasMaxLength(100);

                player.Property(p => p.IsInjured)
                    .HasDefaultValue(false);

                player.HasOne(p => p.Team)
                    .WithMany(t => t.Players)
                    .HasForeignKey(p => p.TeamId)
                    .OnDelete(DeleteBehavior.Restrict);

                player.HasOne(p => p.Position)
                    .WithMany(t => t.Players)
                    .HasForeignKey(p => p.PositionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Position>(position =>
            {
                position.HasKey(p => p.PositionId);

                position.Property(p => p.Name)
                    .IsRequired()
                    .IsUnicode()
                    .HasMaxLength(80);
            });

            modelBuilder.Entity<PlayerStatistic>(entity =>
            {
                entity.HasKey(e => new { e.PlayerId, e.GameId });

                entity.HasOne(e => e.Game)
                    .WithMany(g => g.PlayerStatistics)
                    .HasForeignKey(e => e.GameId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Player)
                    .WithMany(p => p.PlayerStatistics)
                    .HasForeignKey(e => e.PlayerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Game>(game =>
            {
                game.HasKey(g => g.GameId);

                game.Property(g => g.Result)
                    .IsRequired()
                    .IsUnicode()
                    .HasMaxLength(10);

                game.HasOne(g => g.HomeTeam)
                    .WithMany(g => g.HomeGames)
                    .HasForeignKey(g => g.HomeTeamId)
                    .OnDelete(DeleteBehavior.Restrict);

                game.HasOne(g => g.AwayTeam)
                    .WithMany(g => g.AwayGames)
                    .HasForeignKey(g => g.AwayTeamId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Bet>(bet =>
            {
                bet.HasKey(b => b.BetId);

                bet.HasOne(b => b.Game)
                .WithMany(g => g.Bets)
                .HasForeignKey(b => b.GameId)
                .OnDelete(DeleteBehavior.Restrict);

                bet.HasOne(b => b.User)
                .WithMany(g => g.Bets)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<User>(user =>
            {
                user.HasKey(u => u.UserId);

                user.Property(u => u.Username)
                    .IsRequired()
                    .IsUnicode()
                    .HasMaxLength(80);

                user.Property(u => u.Password)
                    .IsRequired()
                    .IsUnicode(false)
                    .HasMaxLength(50);

                user.Property(u => u.Email)
                    .IsRequired()
                    .IsUnicode()
                    .HasMaxLength(80);

                user.Property(u => u.Name)
                    .IsRequired(false)
                    .IsUnicode()
                    .HasMaxLength(100);
            });
        }

    }
}
