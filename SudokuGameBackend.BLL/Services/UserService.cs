using AutoMapper;
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
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;

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
            var userDto = mapper.Map<UserDto>(user);
            if (user.CountryCode != null)
            {
                userDto.Country = await GetCountryByCode(user.CountryCode);
            }
            return userDto;
        }

        private async Task<CountryDto> GetCountryByCode(string code)
        {
            using var client = new HttpClient();
            var reponse = await client.GetStringAsync($"https://restcountries.eu/rest/v2/alpha/{code}?fields=nativeName;alpha2Code");
            return JsonConvert.DeserializeObject<CountryDto>(reponse);
        }

        public async Task UpdateUser(string id, UpdateUserInput updateUserInput)
        {
            User user = await unitOfWork.UserRepository.GetAsync(id);
            user.Name = updateUserInput.Name;
            user.CountryCode = updateUserInput.CountryCode;
            unitOfWork.UserRepository.Update(user);
            await unitOfWork.SaveAsync();
        }

        public async Task DeleteUser(string id)
        {
            await unitOfWork.UserRepository.DeleteAsync(id);
            await unitOfWork.SaveAsync();
        }

        public async Task<bool> IsUserNameCanBeUpdated(string userName, string userId)
        {
            return (await unitOfWork.UserRepository
                .FindAsync(user => user.Name.ToLower() == userName.ToLower() && user.Id != userId))
                .Count == 0;
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

        public async Task<Dictionary<int, UserStatsItemDto>> GetUserStats(string id)
        {
            var userStats = new Dictionary<int, UserStatsItemDto>();
            foreach (GameMode gameMode in Enum.GetValues(typeof(GameMode)))
            {
                userStats.Add((int)gameMode, new UserStatsItemDto());
            }

            var duelRatings = await unitOfWork.DuelRatingRepository.FindAsync(rating => rating.UserId == id);
            foreach (var duelRating in duelRatings)
            {
                userStats[(int)duelRating.GameMode].DuelRating = duelRating.Rating;
            }

            var solvingRatings = await unitOfWork.SolvingRatingRepository.FindAsync(rating => rating.UserId == id);
            foreach (var solvingTime in solvingRatings)
            {
                userStats[(int)solvingTime.GameMode].SolvingTime = solvingTime.Time;
            }

            var singleStats = await unitOfWork.SingleStatsRepository.FindAsync(stats => stats.UserId == id);
            foreach (var stats in singleStats)
            {
                userStats[(int)stats.GameMode].SingleGamesStarted = stats.GamesStarted;
            }

            var duelStats = await unitOfWork.DuelStatsRepository.FindAsync(stats => stats.UserId == id);
            foreach (var stats in duelStats)
            {
                var userStatsItem = userStats[(int)stats.GameMode];
                userStatsItem.DuelGamesStarted = stats.GamesStarted;
                userStatsItem.DuelGamesWon = stats.GamesWon;
                userStatsItem.TotalGamesStarted = userStatsItem.SingleGamesStarted + userStatsItem.DuelGamesStarted;
                if (stats.GamesStarted > 0)
                {
                    userStatsItem.DuelGameWinsPercent = (int)((double)stats.GamesWon / stats.GamesStarted * 100);
                }
            }
            return userStats;
        }
    }
}
