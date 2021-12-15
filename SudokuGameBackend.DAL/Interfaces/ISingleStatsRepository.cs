using SudokuGameBackend.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SudokuGameBackend.DAL.Interfaces
{
    public interface ISingleStatsRepository
    {
        ICollection<SingleStats> GetAll();
        Task<List<SingleStats>> GetAllAsync();
        SingleStats Get(string userId, GameMode gameMode);
        Task<SingleStats> GetAsync(string userId, GameMode gameMode);
        ICollection<SingleStats> Find(Expression<Func<SingleStats, bool>> predicate);
        Task<List<SingleStats>> FindAsync(Expression<Func<SingleStats, bool>> predicate);
        void Create(SingleStats singleStats);
        Task CreateAsync(SingleStats singleStats);
        void Update(SingleStats singleStats);
        void Delete(string userId, GameMode gameMode);
        Task DeleteAsync(string userId, GameMode gameMode);
    }
}
