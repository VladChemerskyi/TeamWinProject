using Microsoft.Extensions.Logging;
using SudokuGameBackend.BLL.Interfaces;
using SudokuGameBackend.DAL.Entities;
using SudokuGameBackend.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SudokuGameBackend.BLL.Services
{
    public class StatsService : IStatsService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<StatsService> logger;

        public StatsService(IUnitOfWork unitOfWork, ILogger<StatsService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public async Task SetInitialStats(string userId)
        {
            foreach (GameMode gameMode in Enum.GetValues(typeof(GameMode)))
            {
                var singleStats = await unitOfWork.SingleStatsRepository.GetAsync(userId, gameMode);
                if (singleStats == null)
                {
                    await unitOfWork.SingleStatsRepository.CreateAsync(new SingleStats
                    {
                        UserId = userId,
                        GameMode = gameMode
                    });
                }

                var duelStats = await unitOfWork.DuelStatsRepository.GetAsync(userId, gameMode);
                if (duelStats == null)
                {
                    await unitOfWork.DuelStatsRepository.CreateAsync(new DuelStats
                    {
                        UserId = userId,
                        GameMode = gameMode
                    });
                }
            }
            await unitOfWork.SaveAsync();
            logger.LogDebug($"SetInitialStats success. userId: {userId}");
        }

        public async Task IncrementDuelGamesCount(string userId, GameMode gameMode)
        {
            var stats = await unitOfWork.DuelStatsRepository.GetAsync(userId, gameMode);
            if (stats != null)
            {
                stats.GamesStarted++;
                unitOfWork.DuelStatsRepository.Update(stats);
                await unitOfWork.SaveAsync();
                logger.LogTrace($"IncrementDuelGamesCount success. userId: {userId}");
            }
            else
            {
                logger.LogWarning($"IncrementDuelGamesCount stats is null. userId: {userId}, gameMode: {gameMode}");
            }
        }

        public async Task IncrementSingleGamesCount(string userId, GameMode gameMode)
        {
            var stats = await unitOfWork.SingleStatsRepository.GetAsync(userId, gameMode);
            if (stats != null)
            {
                stats.GamesStarted++;
                unitOfWork.SingleStatsRepository.Update(stats);
                await unitOfWork.SaveAsync();
                logger.LogTrace($"IncrementSingleGamesCount success. userId: {userId}");
            }
            else
            {
                logger.LogWarning($"IncrementSingleGamesCount stats is null. userId: {userId}, gameMode: {gameMode}");
            }
        }

        public async Task IncrementDuelWinsCount(string userId, GameMode gameMode)
        {
            var stats = await unitOfWork.DuelStatsRepository.GetAsync(userId, gameMode);
            if (stats != null)
            {
                stats.GamesWon++;
                unitOfWork.DuelStatsRepository.Update(stats);
                await unitOfWork.SaveAsync();
                logger.LogTrace($"IncrementDuelWinsCount success. userId: {userId}");
            }
            else
            {
                logger.LogWarning($"IncrementDuelWinsCount stats is null. userId: {userId}, gameMode: {gameMode}");
            }
        }
    }
}
