using SudokuGameBackend.DAL.EF;
using SudokuGameBackend.DAL.Entities;
using SudokuGameBackend.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuGameBackend.DAL.Repositories
{
    public class SolvingRatingRepository : ISolvingRatingRepository
    {
        private readonly ApplicationDbContext dbContext;

        public SolvingRatingRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Create(SolvingRating duelRating)
        {
            dbContext.SolvingLeaderboard.Add(duelRating);
        }

        public void Delete(string userId, GameMode gameMode)
        {
            SolvingRating solvingRating = dbContext.SolvingLeaderboard.Find(userId, gameMode);
            if (solvingRating != null)
            {
                dbContext.SolvingLeaderboard.Remove(solvingRating);
            }
        }

        public IEnumerable<SolvingRating> Find(Func<SolvingRating, bool> predicate)
        {
            return dbContext.SolvingLeaderboard.Where(predicate);
        }

        public SolvingRating Get(string userId, GameMode gameMode)
        {
            return dbContext.SolvingLeaderboard.Find(userId, gameMode);
        }

        public IEnumerable<SolvingRating> GetAll()
        {
            return dbContext.SolvingLeaderboard;
        }

        public void Update(SolvingRating solvingRating)
        {
            dbContext.SolvingLeaderboard.Update(solvingRating);
        }
    }
}
