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
            logger.LogDebug($"CreateSession. gameMode: {gameMode}, userIds: {string.Join(" ", userIds)}");
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var session = new GameSession(gameMode, OnSessionAborted, OnSessionEnded, userIds);
            stopWatch.Stop();
            sessions[session.Id] = session;
            logger.LogDebug($"CreateSession success. sessionId: {session.Id}, gameMode: {gameMode}, " +
                $"creationTime: {Math.Round(stopWatch.Elapsed.TotalSeconds, 2)}s, userIds: {string.Join(" ", userIds)}");
            return session.Id;
        }

        private async Task OnSessionAborted(GameSession session)
        {
            try
            {
                logger.LogDebug($"Session aborted. sessionId: {session.Id}");
                using var scope = serviceScopeFactory.CreateScope();
                var gameHubContext = scope.ServiceProvider.GetService<IHubContext<GameHub>>();
                foreach (var userId in session.UserIds)
                {
                    var userConnectionId = session.GetUserConnectionId(userId);
                    if (userConnectionId != null)
                    {
                        await gameHubContext.Clients.Client(userConnectionId).SendAsync("GameAborted");
                        logger.LogTrace($"GameAborted called. userId: {userId}, sessionId: {session.Id}");
                    }
                }
                this.DeleteSession(session.Id);
            }
            catch (Exception ex)
            {
                logger.LogWarning($"OnSessionAborted exception: {ex}");
            }
        }

        private async Task OnSessionEnded(GameSession session)
        {
            try
            {
                logger.LogDebug($"Session ended. sessionId: {session.Id}");
                using var scope = serviceScopeFactory.CreateScope();
                var ratingService = scope.ServiceProvider.GetService<IRatingService>();
                var gameHubContext = scope.ServiceProvider.GetService<IHubContext<GameHub>>();
                session.Semaphore.WaitOne();
                if (!session.HasResult)
                {
                    await session.CreateTimeExpiredResult(ratingService);
                }
                foreach (var userId in session.UserIds)
                {
                    if (!session.UserResultWasCreated(userId))
                    {
                        var gameResult = await session.GetGameResult(userId, ratingService);
                        logger.LogDebug($"User result. userId: {userId}, sessionId: {session.Id}, GameResult: {gameResult}");
                        await gameHubContext.Clients.Client(session.GetUserConnectionId(userId)).SendAsync("GameTimeExpired", gameResult);
                        logger.LogTrace($"GameTimeExpired sent. userId: {userId}, sessionId: {session.Id}, gameResult: {gameResult}");
                    }
                }
                session.Semaphore.Release();
                this.DeleteSession(session.Id);
            }
            catch (Exception ex)
            {
                logger.LogWarning($"OnSessionEnded exception: {ex}");
            }
        }

        public void DeleteSession(string sessionId)
        {
            if(sessions.TryRemove(sessionId, out var session))
            {
                session.Dispose();
                logger.LogDebug($"Session deleted. sessionId: {sessionId}");
            }
            else
            {
                logger.LogWarning($"Unable to delete session. sessionId: {sessionId}");
            }
        }

        public bool TryGetSession(string sessionId, out GameSession gameSession)
        {
            logger.LogTrace($"TryGetSession. sessionId: {sessionId}");
            return sessions.TryGetValue(sessionId, out gameSession);
        }
    }
}
