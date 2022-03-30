namespace Employees.Data
{
    using Employees.Models;
    using Microsoft.EntityFrameworkCore;

    public class EmployeesDbContext : DbContext
    {
        public EmployeesDbContext()
        { }
        public EmployeesDbContext(DbContextOptions options)
        : base(options) { } 
        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .IsUnicode()
                    .HasMaxLength(60);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .IsUnicode()
                    .HasMaxLength(60);

                entity.Property(e => e.Birthday)
                    .IsRequired(false)
                    .IsUnicode(false)
                    .HasColumnType("DATE");

                entity.Property(e => e.Address)
                    .IsRequired(false)
                    .IsUnicode(false)
                    .HasMaxLength(250);
            });
        }
    }
}
