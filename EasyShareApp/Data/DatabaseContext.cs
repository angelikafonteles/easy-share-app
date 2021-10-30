using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyShareApp.Models;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace EasyShareApp.Data
{
    public partial class DatabaseContext : DbContext
    {
        public DatabaseContext()
        {
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<Document> Document { get; set; }
        public DbSet<Register> Register { get; set; }
        /*
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("server=us-cdbr-east-04.cleardb.com;database=heroku_b5fcb6e5595165d;user=bf74caefbf3b42;password=8bd994f0;SslMode=None",
                   mySqlOptions =>
                   {
                       mySqlOptions.ServerVersion(new Version(5, 6, 50), ServerType.MySql)
                       .EnableRetryOnFailure(
                       maxRetryCount: 10,
                       maxRetryDelay: TimeSpan.FromSeconds(30),
                       errorNumbersToAdd: null);
                   });
        }
        */
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>(entity =>
            {
                //entity.Property(i => i.Id).HasMaxLength(196);
                entity.Property(i => i.InstantCreation).HasColumnType("datetime");
                entity.Property(i => i.InstantExpiration).HasColumnType("datetime");
            });
            modelBuilder.Entity<Register>();

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
