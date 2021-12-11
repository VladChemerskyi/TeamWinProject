using Microsoft.EntityFrameworkCore;
using SudokuGameBackend.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.DAL.EF
{
    public class ApplicationDbContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<DuelRating> DuelLeaderboard { get; set; }
        public DbSet<SolvingRating> SolvingLeaderboard { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .HasMany(u => u.SolvingRatings)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.DuelRatings)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId);


            modelBuilder.Entity<SolvingRating>()
                .HasKey(r => new { r.UserId, r.GameMode });

            modelBuilder.Entity<SolvingRating>()
                .Property(r => r.Time).IsRequired();


            modelBuilder.Entity<DuelRating>()
                .HasKey(r => new { r.UserId, r.GameMode });

            modelBuilder.Entity<DuelRating>()
                .Property(r => r.Rating).IsRequired();
        }
    }
}
