using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
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

        public MatchmakerHub(
            IMatchmakingService matchmakingService, 
            IUnitOfWork unitOfWork, 
            IGameSessionsService gameSessionsService)
        {
            this.matchmakingService = matchmakingService;
            this.unitOfWork = unitOfWork;
            this.gameSessionsService = gameSessionsService;
        }

        public async Task FindDuelGame(GameMode gameMode)
        {
            var userRating = unitOfWork.DuelRatingRepository.Get(Context.UserIdentifier, gameMode).Rating;
            if (matchmakingService.TryFindOpponent(gameMode, userRating, out string opponentId))
            {
                var sessionId = gameSessionsService.CreateSession(gameMode, Context.UserIdentifier, opponentId);
                await Clients.Caller.SendAsync("GameFound", sessionId);
                await Clients.User(opponentId).SendAsync("GameFound", sessionId);
            }
            else
            {
                matchmakingService.AddToQueue(Context.UserIdentifier, userRating, gameMode);
            }
        }

        public async Task CreateSinglePlayerGame(GameMode gameMode)
        {
            var sessionId = gameSessionsService.CreateSession(gameMode, Context.UserIdentifier);
            await Clients.Caller.SendAsync("GameFound", sessionId);
        }
    }
}
