using SudokuGameBackend.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.DAL.Interfaces
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAll();
        User Get(string userId);
        IEnumerable<User> Find(Func<User, bool> predicate);
        void Create(User user);
        void Update(User user);
        void Delete(string userId);
    }
}
