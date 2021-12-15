using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SudokuGameBackend.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IDuelRatingRepository DuelRatingRepository { get; }
        ISolvingRatingRepository SolvingRatingRepository { get; }
        ISingleStatsRepository SingleStatsRepository { get; }
        IDuelStatsRepository DuelStatsRepository { get; }
        void Save();
        Task SaveAsync();
    }
}
