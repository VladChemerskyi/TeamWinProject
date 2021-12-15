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
using SudokuStandard;
using SudokuGameBackend.BLL.Helpers;

namespace SudokuGameBackend.BLL.Services
{
    public class Ratings : IRatings
    {
        public Dictionary<string, int> OldRatings { get; set; }
        public Dictionary<string, int> NewRatings { get; set; }
    }

    public class RatingService : IRatingService
    {
        private readonly int maxRatingDeltaForGame = 16;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<RatingService> logger;
        private readonly ICacheService cacheService;
        private readonly IMapper mapper;
        private readonly int calibrationGames = 5;
        private readonly int ratingSize = 100;

        public RatingService(IUnitOfWork unitOfWork, ILogger<RatingService> logger, ICacheService cacheService, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.cacheService = cacheService;
            this.mapper = mapper;
        }

        public async Task<IRatings> UpdateUsersRatings(
            string user1Id, GameResultType user1GameResult, string user2Id, GameResultType user2GameResult, GameMode gameMode)
        {
            var user1Rating = await unitOfWork.DuelRatingRepository.GetAsync(user1Id, gameMode);
            var user1OldRating = user1Rating.Rating;
            var user2Rating = await unitOfWork.DuelRatingRepository.GetAsync(user2Id, gameMode);
            var user2OldRating = user2Rating.Rating;

            var user1NewRating = CalculateDuelRating(user1OldRating, user2OldRating, user1GameResult);
            var user2NewRating = CalculateDuelRating(user2OldRating, user1OldRating, user2GameResult);

            user1Rating.Rating = user1NewRating;
            user2Rating.Rating = user2NewRating;

            unitOfWork.DuelRatingRepository.Update(user1Rating);
            logger.LogDebug($"UpdateDuelRating. userId: {user1Id}, gameMode: {gameMode}, oldRating: {user1OldRating}, newRating: {user1NewRating}");
            unitOfWork.DuelRatingRepository.Update(user2Rating);
            logger.LogDebug($"UpdateDuelRating. userId: {user2Id}, gameMode: {gameMode}, oldRating: {user2OldRating}, newRating: {user2NewRating}");
            await unitOfWork.SaveAsync();

            return new Ratings
            {
                OldRatings = new Dictionary<string, int>
                {
                    { user1Id, user1OldRating },
                    { user2Id, user2OldRating },
                },
                NewRatings = new Dictionary<string, int>
                {
                    { user1Id, user1NewRating },
                    { user2Id, user2NewRating },
                }
            };
        }

        int CalculateDuelRating(int userRating, int opponentRating, GameResultType userGameResult)
        {
            double points = userGameResult switch
            {
                GameResultType.Victory => 1,
                GameResultType.VictoryByCompletionPercent => 0.75,
                GameResultType.Draw => 0.5,
                GameResultType.DefeatByCompletionPercent => 0.25,
                GameResultType.Defeat => 0,
                _ => throw new RatingServiceException($"Invalid gameResultType value: {userGameResult}"),
            };
            return CalculateUserRating(userRating, opponentRating, points);
        }

        private int CalculateUserRating(int currentUserRating, int currentOpponentRating, double points)
        {
            double expected = 1 / (1 + Math.Pow(10, (currentOpponentRating - currentUserRating) / 400d));
            var ratingDelta = new RatingDeltaMatcher().GetRatingDelta(currentUserRating);
            int newUserRating = currentUserRating + (int)Math.Round(ratingDelta * (points - expected));
            return newUserRating < 0 ? 0 : newUserRating;
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
                        Rating = 1000
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
            int userBestSolvingTime = -1;
            var userRating = await unitOfWork.SolvingRatingRepository.GetAsync(userId, gameMode);
            if (userRating != null)
            {
                userBestSolvingTime = userRating.Time;
            }
            return userBestSolvingTime;
        }

        public async Task<LeaderboardDto> GetDuelLeaderboardAsync(GetLeaderboardInput input, string userId)
        {
            var leaderboards = await cacheService.GetOrCreateAsync(CacheKeys.DuelRating,
                TimeSpan.FromMinutes(1), CreateDuelRatingCacheAsync);
            return await GetLeaderboardDto(userId, input, leaderboards, true);
        }

        public async Task<LeaderboardDto> GetSolvingLeaderboardAsync(GetLeaderboardInput input, string userId)
        {
            var leaderboards = await cacheService.GetOrCreateAsync(CacheKeys.SolvingRating,
                TimeSpan.FromMinutes(1), CreateSolvingRatingCacheAsync);
            return await GetLeaderboardDto(userId, input, leaderboards, false);
        }

        private async Task<LeaderboardDto> GetLeaderboardDto(
            string userId, 
            GetLeaderboardInput input, 
            ConcurrentDictionary<GameMode, List<RatingDto>> leaderboards,
            bool includeCalibrationInfo)
        {
            var leaderboard = leaderboards[input.GameMode.Value];
            var ratings = leaderboard.Take(ratingSize).ToList();
            RatingDto currentPlace = null;
            int? calibrationGamesLeft = null;
            if (userId != null)
            {
                currentPlace = leaderboard.FirstOrDefault(r => r.Id == userId);
                if (currentPlace == null && includeCalibrationInfo)
                {
                    calibrationGamesLeft = await GetCalibrationGames(userId, input.GameMode.Value);
                }
            }
            return new LeaderboardDto
            {
                Ratings = ratings,
                CurrentPlace = currentPlace,
                CalibrationGamesLeft = calibrationGamesLeft
            };
        }

        private async Task<ConcurrentDictionary<GameMode, List<RatingDto>>> CreateDuelRatingCacheAsync()
        {
            var duelStats = await unitOfWork.DuelStatsRepository.FindAsync(stats => stats.GamesStarted >= calibrationGames);
            var duelRatingValues = (await unitOfWork.DuelRatingRepository.GetAllAsync())
                .Where(r => duelStats.Exists(s => s.UserId == r.UserId && s.GameMode == r.GameMode))
                .OrderByDescending(r => r.Rating);
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
                var ratingDto = mapper.Map<RatingDto>(rating);
                ratingDto.Place = gameModsRatings[gameMode].Count + 1;
                gameModsRatings[gameMode].Add(ratingDto);
            }
            return gameModsRatings;
        }

        private async Task<int?> GetCalibrationGames(string userId, GameMode gameMode)
        {
            var stats = await unitOfWork.DuelStatsRepository.GetAsync(userId, gameMode);
            var gamesLeft = calibrationGames - stats.GamesStarted;
            if (gamesLeft <= 0)
            {
                return null;
            }
            return gamesLeft;
        }
    }

    class RatingDeltaMatcher
    {
        private readonly Dictionary<RatingRange, int> ratingDeltaDict = new Dictionary<RatingRange, int>
        {
            { new RatingRange(0, 600), 25 },
            { new RatingRange(601, 2400), 15 },
            { new RatingRange(2401, 3000), 10 },
            { new RatingRange(3001, int.MaxValue), 5 },
        };

        public int GetRatingDelta(int rating)
        {
            return ratingDeltaDict.First(pair => pair.Key.Matches(rating)).Value;
        }
    }
}
