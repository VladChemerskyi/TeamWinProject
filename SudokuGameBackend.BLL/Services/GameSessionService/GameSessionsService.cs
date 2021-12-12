using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SudokuGameBackend.BLL.Hubs;
using SudokuGameBackend.BLL.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SudokuGameBackend.BLL.Services
{
    public class GameSessionsService : IGameSessionsService
    {
        private readonly ConcurrentDictionary<string, GameSession> sessions;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<GameSessionsService> logger;

        public GameSessionsService(IServiceScopeFactory serviceScopeFactory, ILogger<GameSessionsService> logger)
        {
            sessions = new ConcurrentDictionary<string, GameSession>();
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        // TODO: Make things async.
        public string CreateSession(GameMode gameMode, params string[] userIds)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var session = new GameSession(gameMode, userIds);
            stopWatch.Stop();
            session.SetOnSessionEndAction(async () => await OnSessionEnd(session));
            sessions[session.Id] = session;
            logger.LogDebug($"Session created. sessionId: {session.Id}, gameMode: {gameMode}, " + 
                $"creationTime: {Math.Round(stopWatch.Elapsed.TotalSeconds, 2)}s, userIds: {string.Join(" ", userIds)}");
            return session.Id;
        }

        private async Task OnSessionEnd(GameSession session)
        {
            if (session.UserIds.Count <= 2)
            {
                using var scope = serviceScopeFactory.CreateScope();
                var ratingService = scope.ServiceProvider.GetService<IRatingService>();
                var gameHubContext = scope.ServiceProvider.GetService<IHubContext<GameHub>>();
                foreach (var userId in session.UserIds)
                {
                    if (session.GetUserTime(userId) == 0)
                    {
                        int newRating = await ratingService.RemoveDuelRatingForInactivity(userId, session.GameMode);
                        await gameHubContext.Clients.Client(session.GetUserConnectionId(userId)).SendAsync("GameTimeExpired", newRating);
                        logger.LogTrace($"GameTimeExpired celled. userId: {userId}, sessionId: {session.Id}, newRating: {newRating}");
                    }
                }
            }
            this.DeleteSession(session.Id);
        }

        public void DeleteSession(string sessionId)
        {
            sessions.TryRemove(sessionId, out _);
            logger.LogDebug($"Session deleted. sessionId: {sessionId}");
        }

        public bool TryGetSession(string sessionId, out GameSession gameSession)
        {
            logger.LogTrace($"TryGetSession. sessionId: {sessionId}");
            return sessions.TryGetValue(sessionId, out gameSession);
        }
    }
}
