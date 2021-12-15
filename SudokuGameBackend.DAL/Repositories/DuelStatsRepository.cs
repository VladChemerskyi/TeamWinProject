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
    public class DuelStatsRepository : IDuelStatsRepository
    {
        private readonly ApplicationDbContext dbContext;

        public DuelStatsRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Create(DuelStats duelStats)
        {
            dbContext.DuelStats.Add(duelStats);
        }

        public Task CreateAsync(DuelStats duelStats)
        {
            return dbContext.DuelStats.AddAsync(duelStats).AsTask();
        }

        public void Delete(string userId, GameMode gameMode)
        {
            DuelStats duelStats = dbContext.DuelStats.Find(userId, gameMode);
            if (duelStats != null)
            {
                dbContext.DuelStats.Remove(duelStats);
            }
        }

        public async Task DeleteAsync(string userId, GameMode gameMode)
        {
            DuelStats duelStats = await dbContext.DuelStats.FindAsync(userId, gameMode);
            if (duelStats != null)
            {
                dbContext.DuelStats.Remove(duelStats);
            }
        }

        public ICollection<DuelStats> Find(Expression<Func<DuelStats, bool>> predicate)
        {
            return dbContext.DuelStats.Where(predicate).ToList();
        }

        public Task<List<DuelStats>> FindAsync(Expression<Func<DuelStats, bool>> predicate)
        {
            return dbContext.DuelStats.Where(predicate).ToListAsync();
        }

        public DuelStats Get(string userId, GameMode gameMode)
        {
            return dbContext.DuelStats.Find(userId, gameMode);
        }

        public ICollection<DuelStats> GetAll()
        {
            return dbContext.DuelStats.ToList();
        }

        public Task<List<DuelStats>> GetAllAsync()
        {
            return dbContext.DuelStats.ToListAsync();
        }

        public Task<DuelStats> GetAsync(string userId, GameMode gameMode)
        {
            return dbContext.DuelStats.FindAsync(userId, gameMode).AsTask();
        }

        public void Update(DuelStats duelStats)
        {
            dbContext.DuelStats.Update(duelStats);
        }
    }
}
