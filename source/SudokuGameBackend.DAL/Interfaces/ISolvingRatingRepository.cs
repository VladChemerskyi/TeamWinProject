using SudokuGameBackend.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.DAL.Interfaces
{
    public interface ISolvingRatingRepository
    {
        IEnumerable<SolvingRating> GetAll();
        SolvingRating Get(string userId, GameMode gameMode);
        IEnumerable<SolvingRating> Find(Func<SolvingRating, bool> predicate);
        void Create(SolvingRating solvingRating);
        void Update(SolvingRating solvingRating);
        void Delete(string userId, GameMode gameMode);
    }
}
