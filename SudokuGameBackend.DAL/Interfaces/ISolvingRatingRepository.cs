using SudokuGameBackend.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SudokuGameBackend.DAL.Interfaces
{
    public interface ISolvingRatingRepository
    {
        void Create(SolvingRating solvingRating);
        Task CreateAsync(SolvingRating solvingRating);
        void Delete(string userId, GameMode gameMode);
        Task DeleteAsync(string userId, GameMode gameMode);
        IEnumerable<SolvingRating> Find(Expression<Func<SolvingRating, bool>> predicate);
        Task<ICollection<SolvingRating>> FindAsync(Expression<Func<SolvingRating, bool>> predicate);
        SolvingRating Get(string userId, GameMode gameMode);
        ICollection<SolvingRating> GetAll();
        Task<ICollection<SolvingRating>> GetAllAsync();
        Task<SolvingRating> GetAsync(string userId, GameMode gameMode);
        void Update(SolvingRating solvingRating);
    }
}
