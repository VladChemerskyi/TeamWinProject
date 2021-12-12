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
    public class DuelRatingRepository : IDuelRatingRepository
    {
        private readonly ApplicationDbContext dbContext;

        public DuelRatingRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Create(DuelRating duelRating)
        {
            dbContext.DuelLeaderboard.Add(duelRating);
        }

        public async Task CreateAsync(DuelRating duelRating)
        {
            await dbContext.DuelLeaderboard.AddAsync(duelRating);
        }

        public void Delete(string userId, GameMode gameMode)
        {
            DuelRating duelRating = dbContext.DuelLeaderboard.Find(userId, gameMode);
            if (duelRating != null)
            {
                dbContext.DuelLeaderboard.Remove(duelRating);
            }
        }

        public async Task DeleteAsync(string userId, GameMode gameMode)
        {
            DuelRating duelRating = await dbContext.DuelLeaderboard.FindAsync(userId, gameMode);
            if (duelRating != null)
            {
                dbContext.DuelLeaderboard.Remove(duelRating);
            }
        }

        public ICollection<DuelRating> Find(Expression<Func<DuelRating, bool>> predicate)
        {
            return dbContext.DuelLeaderboard.Where(predicate).ToList();
        }

        public async Task<ICollection<DuelRating>> FindAsync(Expression<Func<DuelRating, bool>> predicate)
        {
            return await dbContext.DuelLeaderboard.Where(predicate).ToListAsync();
        }

        public DuelRating Get(string userId, GameMode gameMode)
        {
            return dbContext.DuelLeaderboard.Find(userId, gameMode);
        }

        public async Task<DuelRating> GetAsync(string userId, GameMode gameMode)
        {
            return await dbContext.DuelLeaderboard.FindAsync(userId, gameMode);
        }

        public ICollection<DuelRating> GetAll()
        {
            return dbContext.DuelLeaderboard.Include(r => r.User).ToList();
        }

        public async Task<ICollection<DuelRating>> GetAllAsync()
        {
            return await dbContext.DuelLeaderboard.Include(r => r.User).ToListAsync();
        }

        public void Update(DuelRating duelRating)
        {
            dbContext.DuelLeaderboard.Update(duelRating);
        }
    }
}
