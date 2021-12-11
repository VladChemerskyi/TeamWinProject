using SudokuGameBackend.DAL.Interfaces;
using SudokuGameBackend.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SudokuGameBackend.DAL.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext dbContext;
        private IUserRepository userRepository;
        private IDuelRatingRepository duelRatingRepository;
        private ISolvingRatingRepository solvingRatingRepository;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IUserRepository UserRepository
        {
            get
            {
                if (userRepository == null)
                {
                    userRepository = new UserRepository(dbContext);
                }
                return userRepository;
            }
        }

        public IDuelRatingRepository DuelRatingRepository
        {
            get
            {
                if (duelRatingRepository == null)
                {
                    duelRatingRepository = new DuelRatingRepository(dbContext);
                }
                return duelRatingRepository;
            }
        }

        public ISolvingRatingRepository SolvingRatingRepository
        {
            get
            {
                if (solvingRatingRepository == null)
                {
                    solvingRatingRepository = new SolvingRatingRepository(dbContext);
                }
                return solvingRatingRepository;
            }
        }

        public void Save()
        {
            dbContext.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
