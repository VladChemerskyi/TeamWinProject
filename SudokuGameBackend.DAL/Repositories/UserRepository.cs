using Microsoft.EntityFrameworkCore;
using SudokuGameBackend.DAL.EF;
using SudokuGameBackend.DAL.Entities;
using SudokuGameBackend.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public void Delete(string userId)
        {
            User user = dbContext.Users.Find(userId);
            if (user != null)
            {
                dbContext.Users.Remove(user);
            }
        }

        public IEnumerable<User> Find(Func<User, bool> predicate)
        {
            return dbContext.Users.Where(predicate);
        }

        public User Get(string userId)
        {
            return dbContext.Users.Find(userId);
        }

        public IEnumerable<User> GetAll()
        {
            return dbContext.Users;
        }

        public void Update(User user)
        {
            dbContext.Users.Update(user);
        }
    }
}
