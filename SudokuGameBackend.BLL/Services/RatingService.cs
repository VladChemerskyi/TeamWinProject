using Microsoft.Extensions.Logging;
using SudokuGameBackend.BLL.Exceptions;
using SudokuGameBackend.BLL.Interfaces;
using SudokuGameBackend.DAL.Entities;
using SudokuGameBackend.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using SudokuGameBackend.BLL.DTO;
using System.Collections.Concurrent;
using AutoMapper;
using SudokuGameBackend.BLL.InputModels;

namespace SudokuGameBackend.BLL.Services
{
    public class RatingService : IRatingService
    {
        private readonly int maxRatingDeltaForGame = 16;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<RatingService> logger;
        private readonly ICacheService cacheService;
        private readonly IMapper mapper;

        public RatingService(IUnitOfWork unitOfWork, ILogger<RatingService> logger, ICacheService cacheService, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.cacheService = cacheService;
            this.mapper = mapper;
        }

        public async Task<Dictionary<string, int>> CalculateDuelRatings(string winnerId, string loserId, GameMode gameMode)
        {
            var winnerRating = await unitOfWork.DuelRatingRepository.GetAsync(winnerId, gameMode);
            var loserRating = await unitOfWork.DuelRatingRepository.GetAsync(loserId, gameMode);

            int newWinnerRating = CalculateNewWinnerRating(winnerRating.Rating, loserRating.Rating);
            int newLoserRating = CalculateNewLoserRating(loserRating.Rating, winnerRating.Rating);

            return new Dictionary<string, int>
            {
                { winnerId, newWinnerRating },
                { loserId,  newLoserRating  }
            };
        }

        public async Task UpdateDuelRating(string userId, GameMode gameMode, int rating)
        {
            var duelRating = await unitOfWork.DuelRatingRepository.GetAsync(userId, gameMode);
            logger.LogDebug($"UpdateDuelRating. userId: {userId}, gameMode: {gameMode}, oldRating: {duelRating.Rating}, newRating: {rating}");
            duelRating.Rating = rating;
            unitOfWork.DuelRatingRepository.Update(duelRating);
            await unitOfWork.SaveAsync();
            logger.LogTrace($"UpdateDuelRating success. userId: {userId}, gameMode: {gameMode}");
        }

        private int CalculateUserRating(int currentUserRating, int currentOpponentRating, bool isWinner)
        {
            double expected = 1 / (1 + Math.Pow(10, (currentOpponentRating - currentUserRating) / 400d));
            int newUserRating = currentUserRating + (int)Math.Round(maxRatingDeltaForGame * ((isWinner ? 1 : 0) - expected));
            return newUserRating;
        }

        private int CalculateNewWinnerRating(int winnerRating, int loserRating)
        {
            return CalculateUserRating(winnerRating, loserRating, true);
        }

        private int CalculateNewLoserRating(int loserRating, int winnerRating)
        {
            return CalculateUserRating(loserRating, winnerRating, false);
        }

        public async Task<bool> UpdateSolvingRating(string userId, int time, GameMode gameMode)
        {
            bool isNewBest = false;
            var rating = await unitOfWork.SolvingRatingRepository.GetAsync(userId, gameMode);
            if (rating != null)
            {
                if (time < rating.Time)
                {
                    isNewBest = true;
                    logger.LogDebug($"UpdateSolvingRating. userId: {userId}, gameMode: {gameMode}, oldTime: {rating.Time}, newTime: {time}");
                    rating.Time = time;
                }
                unitOfWork.SolvingRatingRepository.Update(rating);
            }
            else
            {
                isNewBest = true;
                logger.LogDebug($"UpdateSolvingRating. userId: {userId}, gameMode: {gameMode}, newTime: {time}");
                await unitOfWork.SolvingRatingRepository.CreateAsync(new SolvingRating
                {
                    UserId = userId,
                    GameMode = gameMode,
                    Time = time
                });
            }
            await unitOfWork.SaveAsync();
            logger.LogTrace($"UpdateSolvingRating success. userId: {userId}, gameMode: {gameMode}, isNewBest: {isNewBest}");
            return isNewBest;
        }

        public async Task SetInitialDuelRating(string userId)
        {
            foreach (GameMode gameMode in Enum.GetValues(typeof(GameMode)))
            {
                var rating = await unitOfWork.DuelRatingRepository.GetAsync(userId, gameMode);
                if (rating == null)
                {
                    await unitOfWork.DuelRatingRepository.CreateAsync(new DuelRating
                    {
                        UserId = userId,
                        GameMode = gameMode,
                        Rating = 1400
                    });
                }
            }
            await unitOfWork.SaveAsync();
            logger.LogDebug($"SetInitialDuelRating success. userId: {userId}");
        }

        public async Task<int> RemoveDuelRatingForInactivity(string userId, GameMode gameMode)
        {
            var currentRating = await unitOfWork.DuelRatingRepository.GetAsync(userId, gameMode);
            currentRating.Rating -= maxRatingDeltaForGame;
            unitOfWork.DuelRatingRepository.Update(currentRating);
            await unitOfWork.SaveAsync();
            return currentRating.Rating;
        }

        public async Task<int> GetUserRating(string userId, GameMode gameMode)
        {
            var userRating = await unitOfWork.DuelRatingRepository.GetAsync(userId, gameMode);
            if (userRating == null)
            {
                throw new ItemNotFoundException($"Rating not found. userId: {userId}, gameMode: {gameMode}");
            }
            return userRating.Rating;
        }

        public async Task<int> GetUserSolvingRating(string userId, GameMode gameMode)
        {
            int userBestSolvingTime = 0;
            var userRating = await unitOfWork.SolvingRatingRepository.GetAsync(userId, gameMode);
            if (userRating != null)
            {
                userBestSolvingTime = userRating.Time;
            }
            return userBestSolvingTime;
        }

        public async Task<List<RatingDto>> GetDuelLeaderboardAsync(GetLeaderboardInput input)
        {
            var leaderboards = await cacheService.GetOrCreateAsync(CacheKeys.DuelRating,
                TimeSpan.FromMinutes(1), CreateDuelRatingCacheAsync);
            return leaderboards[input.GameMode.Value].Skip(input.Index).Take(input.Count).ToList();
        }

        public async Task<List<RatingDto>> GetSolvingLeaderboardAsync(GetLeaderboardInput input)
        {
            var leaderboards = await cacheService.GetOrCreateAsync(CacheKeys.SolvingRating,
                TimeSpan.FromMinutes(1), CreateSolvingRatingCacheAsync);
            return leaderboards[input.GameMode.Value].Skip(input.Index).Take(input.Count).ToList();
        }

        private async Task<ConcurrentDictionary<GameMode, List<RatingDto>>> CreateDuelRatingCacheAsync()
        {
            var duelRatingValues = (await unitOfWork.DuelRatingRepository.GetAllAsync()).OrderByDescending(r => r.Rating);
            return CreateRatingCache(duelRatingValues, r => r.GameMode);
        }

        private async Task<ConcurrentDictionary<GameMode, List<RatingDto>>> CreateSolvingRatingCacheAsync()
        {
            var duelRatingValues = (await unitOfWork.SolvingRatingRepository.GetAllAsync()).OrderBy(r => r.Time);
            return CreateRatingCache(duelRatingValues, r => r.GameMode);
        }

        private ConcurrentDictionary<GameMode, List<RatingDto>> CreateRatingCache<T>(
            IEnumerable<T> sortedValues, 
            Func<T, GameMode> gameModeSelector)
        {
            var gameModsRatings = new ConcurrentDictionary<GameMode, List<RatingDto>>();
            foreach (GameMode gameMode in Enum.GetValues(typeof(GameMode)))
            {
                gameModsRatings.TryAdd(gameMode, new List<RatingDto>());
            }
            foreach (var rating in sortedValues)
            {
                var gameMode = gameModeSelector.Invoke(rating);
                gameModsRatings[gameMode].Add(mapper.Map<RatingDto>(rating));
            }
            return gameModsRatings;
        }
    }
}
