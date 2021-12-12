using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SudokuGameBackend.BLL.Interfaces;
using SudokuGameBackend.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SudokuGameBackend.BLL.Hubs
{
    [Authorize]
    public class MatchmakerHub : Hub
    {
        private readonly IMatchmakingService matchmakingService;
        private readonly IUnitOfWork unitOfWork;
        private readonly IGameSessionsService gameSessionsService;
        private readonly ILogger<MatchmakerHub> logger;

        public MatchmakerHub(
            IMatchmakingService matchmakingService, 
            IUnitOfWork unitOfWork, 
            IGameSessionsService gameSessionsService,
            ILogger<MatchmakerHub> logger)
        {
            this.matchmakingService = matchmakingService;
            this.unitOfWork = unitOfWork;
            this.gameSessionsService = gameSessionsService;
            this.logger = logger;
        }

        public async Task FindDuelGame(GameMode gameMode)
        {
            logger.LogDebug($"FindDuelGame. userId: {Context.UserIdentifier}, gameMode: {gameMode}");
            // TODO: Check gameMode.
            var userRating = (await unitOfWork.DuelRatingRepository.GetAsync(Context.UserIdentifier, gameMode)).Rating;
            if (matchmakingService.TryFindOpponent(gameMode, userRating, Context.UserIdentifier, out string opponentId))
            {
                var sessionId = gameSessionsService.CreateSession(gameMode, Context.UserIdentifier, opponentId);
                await Clients.Caller.SendAsync("GameFound", sessionId);
                logger.LogTrace($"Duel.GameFound sent. userId: {Context.UserIdentifier}, sessionId: {sessionId}");
                await Clients.User(opponentId).SendAsync("GameFound", sessionId);
                logger.LogTrace($"Duel.GameFound sent. userId: {opponentId}, sessionId: {sessionId}");
            }
            else
            {
                matchmakingService.AddToQueue(Context.UserIdentifier, userRating, gameMode);
            }
        }

        public async Task RemoveFromQueue()
        {
            logger.LogDebug($"RemoveFromQueue. userId: {Context.UserIdentifier}");
            await Task.Run(() => matchmakingService.RemoveFromQueue(Context.UserIdentifier));
        }

        public async Task CreateSinglePlayerGame(GameMode gameMode)
        {
            logger.LogDebug($"CreateSinglePlayerGame. userId: {Context.UserIdentifier}, gameMode: {gameMode}");
            var sessionId = gameSessionsService.CreateSession(gameMode, Context.UserIdentifier);
            await Clients.Caller.SendAsync("GameFound", sessionId);
            logger.LogTrace($"Single.GameFound sent. userId: {Context.UserIdentifier}, sessionId: {sessionId}");
        }
    }
}
