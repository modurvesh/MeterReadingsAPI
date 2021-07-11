using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeterReadingsAPI.Models
{
    public class MeterReadingsContext : DbContext
    {
        public MeterReadingsContext()
        {
            // if the db is not created, create it and populate with test data
            if (!SQLConfig.TempDBCreated)
            {
                SQLConfig.TempDBCreated = true;

                Database.EnsureDeleted();
                Database.EnsureCreated();
                SQLConfig.PopulateUserAccounts();
            }
        }

        public MeterReadingsContext(DbContextOptions<MeterReadingsContext> options) : base(options)
        {

        }

        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<MeterReading> MeterReadings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                 optionsBuilder.UseSqlite(SQLConfig.connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserAccount>(entity =>
            {
                entity.HasKey(e => e.AccountId);
            });
        }
    }
}
