using SudokuGameBackend.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SudokuGameBackend.DAL.Interfaces
{
    public interface IDuelRatingRepository
    {
        ICollection<DuelRating> GetAll();
        Task<ICollection<DuelRating>> GetAllAsync();
        DuelRating Get(string userId, GameMode gameMode);
        Task<DuelRating> GetAsync(string userId, GameMode gameMode);
        ICollection<DuelRating> Find(Expression<Func<DuelRating, bool>> predicate);
        Task<ICollection<DuelRating>> FindAsync(Expression<Func<DuelRating, bool>> predicate);
        void Create(DuelRating duelRating);
        Task CreateAsync(DuelRating duelRating);
        void Update(DuelRating duelRating);
        void Delete(string userId, GameMode gameMode);
        Task DeleteAsync(string userId, GameMode gameMode);
    }
}
