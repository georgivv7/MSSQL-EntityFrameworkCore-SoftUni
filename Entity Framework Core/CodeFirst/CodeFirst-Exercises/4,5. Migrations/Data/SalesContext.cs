namespace P03_SalesDatabase.Data
{
    using Microsoft.EntityFrameworkCore;
    using P03_SalesDatabase.Data.Models;
    public class SalesContext : DbContext
    {
        public SalesContext()
        { }
        public SalesContext(DbContextOptions options)
        : base(options) { }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Sale> Sales { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId);

                entity.Property(e => e.Name)
                      .IsRequired(true)
                      .IsUnicode(true)
                      .HasMaxLength(50);

                entity.Property(e => e.Description)
                     .IsRequired(false)
                     .IsUnicode(true)
                     .HasMaxLength(250)
                     .HasDefaultValue("No description");

                entity.Property(e => e.Quantity)
                      .IsRequired(true);

                entity.Property(e => e.Price)
                      .IsRequired(true);

                //entity.HasMany(e => e.Sales)
                //      .WithOne(p => p.Product)
                //      .HasForeignKey(e => e.ProductId);                
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerId);

                entity.Property(e => e.Name)
                      .IsRequired(true)
                      .IsUnicode(true)
                      .HasMaxLength(100);

                entity.Property(e => e.Email)
                      .IsRequired(true)
                      .IsUnicode(false)
                      .HasMaxLength(80);

                entity.Property(e => e.CreditCardNumber)
                      .IsRequired(true)
                      .IsUnicode(false);

                //entity.HasMany(e => e.Sales)
                //      .WithOne(p => p.Customer)
                //      .HasForeignKey(e => e.CustomerId);
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.HasKey(e => e.StoreId);

                entity.Property(e => e.Name)
                      .IsRequired(true)
                      .IsUnicode(true)
                      .HasMaxLength(80);
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasKey(e => e.SaleId);

                entity.Property(e => e.Date)
                      .IsRequired(true)
                      .HasColumnType("DATETIME2");

                entity.HasOne(e => e.Product)
                      .WithMany(p => p.Sales)
                      .HasForeignKey(e => e.ProductId);

                entity.HasOne(e => e.Customer)
                      .WithMany(p => p.Sales)
                      .HasForeignKey(e => e.CustomerId);

                entity.HasOne(e => e.Store)
                      .WithMany(p => p.Sales)
                      .HasForeignKey(e => e.StoreId);
            });
        }
    }
}
