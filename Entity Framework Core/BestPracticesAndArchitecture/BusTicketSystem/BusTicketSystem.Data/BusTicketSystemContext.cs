namespace BusTicketSystem.Data
{
    using BusTicketSystem.Data.Configuration;
    using BusTicketSystem.Models;
    using Microsoft.EntityFrameworkCore;
    using System;
    public class BusTicketSystemContext : DbContext
    {
        public BusTicketSystemContext()
        { }
        public BusTicketSystemContext(DbContextOptions options)
        :base(options) { }

        public DbSet<BusCompany> BusCompanies { get; set; }    
        public DbSet<BankAccount> BankAccounts { get; set; }       
        public DbSet<BusStation> BusStations { get; set; }       
        public DbSet<Country> Countries { get; set; }           
        public DbSet<Customer> Customers { get; set; }            
        public DbSet<Review> Reviews { get; set; }              
        public DbSet<Ticket> Tickets { get; set; }     
        public DbSet<Town> Towns { get; set; }     
        public DbSet<Trip> Trips { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BusCompanyConfig());

            modelBuilder.ApplyConfiguration(new BankAccountConfig());

            modelBuilder.ApplyConfiguration(new BusStationConfig());

            modelBuilder.ApplyConfiguration(new CustomerConfig());

            modelBuilder.ApplyConfiguration(new ReviewConfig());

            modelBuilder.ApplyConfiguration(new TicketConfig());

            modelBuilder.ApplyConfiguration(new TownConfig());

            modelBuilder.ApplyConfiguration(new TripConfig());
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Config.ConnectionString);
            }           
        }

        
    }
}
