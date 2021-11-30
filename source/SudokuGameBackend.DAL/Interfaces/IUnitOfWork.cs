using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IDuelRatingRepository DuelRatingRepository { get; }
        ISolvingRatingRepository SolvingRatingRepository { get; }
        void Save();
    }
}
