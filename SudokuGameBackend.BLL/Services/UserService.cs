using AutoMapper;
using SudokuGameBackend.BLL.DTO;
using SudokuGameBackend.BLL.Exceptions;
using SudokuGameBackend.BLL.InputModels;
using SudokuGameBackend.BLL.Interfaces;
using SudokuGameBackend.DAL.Entities;
using SudokuGameBackend.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SudokuGameBackend.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task AddUser(AddUserInput input)
        {
            User user = mapper.Map<User>(input);
            await unitOfWork.UserRepository.CreateAsync(user);
            await unitOfWork.SaveAsync();
        }

        public async Task<UserDto> GetUser(string id)
        {
            User user = await unitOfWork.UserRepository.GetAsync(id);
            if (user == null)
            {
                throw new ItemNotFoundException($"Item not found. Id: {id}");
            }
            return mapper.Map<UserDto>(user);
        }
    }
}
