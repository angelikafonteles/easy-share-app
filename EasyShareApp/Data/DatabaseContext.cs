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
