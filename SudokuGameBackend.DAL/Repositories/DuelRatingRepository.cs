using SudokuGameBackend.DAL.EF;
using SudokuGameBackend.DAL.Entities;
using SudokuGameBackend.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public void Delete(string userId, GameMode gameMode)
        {
            DuelRating duelRating = dbContext.DuelLeaderboard.Find(userId, gameMode);
            if (duelRating != null)
            {
                dbContext.DuelLeaderboard.Remove(duelRating);
            }
        }

        public IEnumerable<DuelRating> Find(Func<DuelRating, bool> predicate)
        {
            return dbContext.DuelLeaderboard.Where(predicate);
        }

        public DuelRating Get(string userId, GameMode gameMode)
        {
            return dbContext.DuelLeaderboard.Find(userId, gameMode);
        }

        public IEnumerable<DuelRating> GetAll()
        {
            return dbContext.DuelLeaderboard;
        }

        public void Update(DuelRating duelRating)
        {
            dbContext.DuelLeaderboard.Update(duelRating);
        }
    }
}
