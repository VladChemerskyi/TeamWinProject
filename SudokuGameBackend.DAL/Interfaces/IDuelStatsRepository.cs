using SudokuGameBackend.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SudokuGameBackend.DAL.Interfaces
{
    public interface IDuelStatsRepository
    {
        ICollection<DuelStats> GetAll();
        Task<List<DuelStats>> GetAllAsync();
        DuelStats Get(string userId, GameMode gameMode);
        Task<DuelStats> GetAsync(string userId, GameMode gameMode);
        ICollection<DuelStats> Find(Expression<Func<DuelStats, bool>> predicate);
        Task<List<DuelStats>> FindAsync(Expression<Func<DuelStats, bool>> predicate);
        void Create(DuelStats duelStats);
        Task CreateAsync(DuelStats duelStats);
        void Update(DuelStats duelStats);
        void Delete(string userId, GameMode gameMode);
        Task DeleteAsync(string userId, GameMode gameMode);
    }
}
