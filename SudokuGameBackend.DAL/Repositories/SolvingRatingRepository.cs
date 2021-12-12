using Microsoft.EntityFrameworkCore;
using SudokuGameBackend.DAL.EF;
using SudokuGameBackend.DAL.Entities;
using SudokuGameBackend.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SudokuGameBackend.DAL.Repositories
{
    public class SolvingRatingRepository : ISolvingRatingRepository
    {
        private readonly ApplicationDbContext dbContext;

        public SolvingRatingRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Create(SolvingRating solvingRating)
        {
            dbContext.SolvingLeaderboard.Add(solvingRating);
        }

        public async Task CreateAsync(SolvingRating solvingRating)
        {
            await dbContext.SolvingLeaderboard.AddAsync(solvingRating);
        }

        public void Delete(string userId, GameMode gameMode)
        {
            SolvingRating solvingRating = dbContext.SolvingLeaderboard.Find(userId, gameMode);
            if (solvingRating != null)
            {
                dbContext.SolvingLeaderboard.Remove(solvingRating);
            }
        }

        public async Task DeleteAsync(string userId, GameMode gameMode)
        {
            SolvingRating solvingRating = await dbContext.SolvingLeaderboard.FindAsync(userId, gameMode);
            if (solvingRating != null)
            {
                dbContext.SolvingLeaderboard.Remove(solvingRating);
            }
        }

        public IEnumerable<SolvingRating> Find(Expression<Func<SolvingRating, bool>> predicate)
        {
            return dbContext.SolvingLeaderboard.Where(predicate);
        }

        public async Task<ICollection<SolvingRating>> FindAsync(Expression<Func<SolvingRating, bool>> predicate)
        {
            return await dbContext.SolvingLeaderboard.Where(predicate).ToListAsync();
        }

        public SolvingRating Get(string userId, GameMode gameMode)
        {
            return dbContext.SolvingLeaderboard.Find(userId, gameMode);
        }

        public async Task<SolvingRating> GetAsync(string userId, GameMode gameMode)
        {
            return await dbContext.SolvingLeaderboard.FindAsync(userId, gameMode);
        }

        public ICollection<SolvingRating> GetAll()
        {
            return dbContext.SolvingLeaderboard.Include(r => r.User).ToList();
        }

        public async Task<ICollection<SolvingRating>> GetAllAsync()
        {
            return await dbContext.SolvingLeaderboard.Include(r => r.User).ToListAsync();
        }

        public void Update(SolvingRating solvingRating)
        {
            dbContext.SolvingLeaderboard.Update(solvingRating);
        }
    }
}
