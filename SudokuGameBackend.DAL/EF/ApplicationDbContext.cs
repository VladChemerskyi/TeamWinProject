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
        public DbSet<SingleStats> SingleStats { get; set; }
        public DbSet<DuelStats> DuelStats { get; set; }

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

            modelBuilder.Entity<User>()
                .Property(u => u.Name).IsRequired();


            modelBuilder.Entity<SolvingRating>()
                .HasKey(r => new { r.UserId, r.GameMode });

            modelBuilder.Entity<SolvingRating>()
                .Property(r => r.Time).IsRequired();


            modelBuilder.Entity<DuelRating>()
                .HasKey(r => new { r.UserId, r.GameMode });

            modelBuilder.Entity<DuelRating>()
                .Property(r => r.Rating).IsRequired();


            modelBuilder.Entity<SingleStats>()
                .HasKey(s => new { s.UserId, s.GameMode });

            modelBuilder.Entity<SingleStats>()
                .Property(s => s.GamesStarted)
                .IsRequired()
                .HasDefaultValue(0);


            modelBuilder.Entity<DuelStats>()
                .HasKey(s => new { s.UserId, s.GameMode });

            modelBuilder.Entity<DuelStats>()
                .Property(s => s.GamesStarted)
                .IsRequired()
                .HasDefaultValue(0);

            modelBuilder.Entity<DuelStats>()
                .Property(s => s.GamesWon)
                .IsRequired()
                .HasDefaultValue(0);
        }
    }
}
