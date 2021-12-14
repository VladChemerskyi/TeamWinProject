﻿using AutoMapper;
using Microsoft.Extensions.Logging;
using SudokuGameBackend.BLL.DTO;
using SudokuGameBackend.BLL.Exceptions;
using SudokuGameBackend.BLL.InputModels;
using SudokuGameBackend.BLL.Interfaces;
using SudokuGameBackend.DAL.Entities;
using SudokuGameBackend.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SudokuGameBackend.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<UserService> logger;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task AddUser(AddUserDto input)
        {
            try
            {
                User user = mapper.Map<User>(input);
                await unitOfWork.UserRepository.CreateAsync(user);
                await unitOfWork.SaveAsync();
                logger.LogDebug($"AddUser success. {input}");
            }
            catch (Exception ex)
            {
                throw new UserAddingException($"User adding exception. Input: {input}", ex);
            }
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

        public async Task<bool> IsUserNameAvailable(string userName)
        {
            return (await unitOfWork.UserRepository
                .FindAsync(user => user.Name.ToLower() == userName.ToLower()))
                .Count == 0;
        }

        public async Task<bool> DoesUserExist(string userId)
        {
            return (await unitOfWork.UserRepository.GetAsync(userId)) != null;
        }

        public string GetNameFromEmail(string email)
        {
            var userName = email.Split('@')[0];
            userName = Regex.Replace(userName, "[^A-Za-z0-9_]+", string.Empty);
            while (userName.Contains("__"))
            {
                userName = userName.Replace("__", "_");
            }
            if (userName.Length > 16)
            {
                userName = userName.Substring(0, 16);
            }
            else if (userName.Length < 3)
            {
                userName = userName.PadRight(3, '0');
            }
            return userName;
        }
    }
}
