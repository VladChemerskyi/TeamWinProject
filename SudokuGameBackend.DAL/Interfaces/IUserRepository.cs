using SudokuGameBackend.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SudokuGameBackend.DAL.Interfaces
{
    public interface IUserRepository
    {
        ICollection<User> GetAll();
        Task<ICollection<User>> GetAllAsync();
        User Get(string userId);
        Task<User> GetAsync(string userId);
        ICollection<User> Find(Expression<Func<User, bool>> predicate);
        Task<ICollection<User>> FindAsync(Expression<Func<User, bool>> predicate);
        void Create(User user);
        Task CreateAsync(User user);
        void Update(User user);
        void Delete(string userId);
        Task DeleteAsync(string userId);
    }
}
