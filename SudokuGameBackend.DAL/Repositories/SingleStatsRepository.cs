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
    public class SingleStatsRepository : ISingleStatsRepository
    {
        private readonly ApplicationDbContext dbContext;

        public SingleStatsRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Create(SingleStats singleStats)
        {
            dbContext.SingleStats.Add(singleStats);
        }

        public Task CreateAsync(SingleStats singleStats)
        {
            return dbContext.SingleStats.AddAsync(singleStats).AsTask();
        }

        public void Delete(string userId, GameMode gameMode)
        {
            SingleStats singleStats = dbContext.SingleStats.Find(userId, gameMode);
            if (singleStats != null)
            {
                dbContext.SingleStats.Remove(singleStats);
            }
        }

        public async Task DeleteAsync(string userId, GameMode gameMode)
        {
            SingleStats singleStats = await dbContext.SingleStats.FindAsync(userId, gameMode);
            if (singleStats != null)
            {
                dbContext.SingleStats.Remove(singleStats);
            }
        }

        public ICollection<SingleStats> Find(Expression<Func<SingleStats, bool>> predicate)
        {
            return dbContext.SingleStats.Where(predicate).ToList();
        }

        public Task<List<SingleStats>> FindAsync(Expression<Func<SingleStats, bool>> predicate)
        {
            return dbContext.SingleStats.Where(predicate).ToListAsync();
        }

        public SingleStats Get(string userId, GameMode gameMode)
        {
            return dbContext.SingleStats.Find(userId, gameMode);
        }

        public ICollection<SingleStats> GetAll()
        {
            return dbContext.SingleStats.ToList();
        }

        public Task<List<SingleStats>> GetAllAsync()
        {
            return dbContext.SingleStats.ToListAsync();
        }

        public Task<SingleStats> GetAsync(string userId, GameMode gameMode)
        {
            return dbContext.SingleStats.FindAsync(userId, gameMode).AsTask();
        }

        public void Update(SingleStats singleStats)
        {
            dbContext.SingleStats.Update(singleStats);
        }
    }
}
