using Microsoft.EntityFrameworkCore;
using SudokuGameBackend.DAL.EF;
using SudokuGameBackend.DAL.Entities;
using SudokuGameBackend.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SudokuGameBackend.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Create(User user)
        {
            dbContext.Users.Add(user);
        }

        public async Task CreateAsync(User user)
        {
            await dbContext.Users.AddAsync(user);
        }

        public void Delete(string userId)
        {
            User user = dbContext.Users.Find(userId);
            if (user != null)
            {
                dbContext.Users.Remove(user);
            }
        }

        public async Task DeleteAsync(string userId)
        {
            User user = await dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                dbContext.Users.Remove(user);
            }
        }

        public ICollection<User> Find(Expression<Func<User, bool>> predicate)
        {
            return dbContext.Users.Where(predicate).ToList();
        }

        public async Task<ICollection<User>> FindAsync(Expression<Func<User, bool>> predicate)
        {
            return await dbContext.Users.Where(predicate).ToListAsync();
        }

        public User Get(string userId)
        {
            return dbContext.Users.Find(userId);
        }

        public async Task<User> GetAsync(string userId)
        {
            return await dbContext.Users.FindAsync(userId);
        }

        public ICollection<User> GetAll()
        {
            return dbContext.Users.ToList();
        }

        public async Task<ICollection<User>> GetAllAsync()
        {
            return await dbContext.Users.ToListAsync();
        }

        public void Update(User user)
        {
            dbContext.Users.Update(user);
        }
    }
}
