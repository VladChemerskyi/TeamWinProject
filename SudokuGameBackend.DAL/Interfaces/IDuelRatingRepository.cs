using SudokuGameBackend.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.DAL.Interfaces
{
    public interface IDuelRatingRepository
    {
        IEnumerable<DuelRating> GetAll();
        DuelRating Get(string userId, GameMode gameMode);
        IEnumerable<DuelRating> Find(Func<DuelRating, bool> predicate);
        void Create(DuelRating duelRating);
        void Update(DuelRating duelRating);
        void Delete(string userId, GameMode gameMode);
    }
}
