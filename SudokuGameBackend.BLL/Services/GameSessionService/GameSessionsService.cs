using Microsoft.Extensions.DependencyInjection;
using SudokuGameBackend.BLL.Interfaces;
using System.Collections.Concurrent;

namespace SudokuGameBackend.BLL.Services
{
    public class GameSessionsService : IGameSessionsService
    {
        private readonly ConcurrentDictionary<string, GameSession> sessions;
        private readonly IServiceScopeFactory serviceScopeFactory;

        public GameSessionsService(IServiceScopeFactory serviceScopeFactory)
        {
            sessions = new ConcurrentDictionary<string, GameSession>();
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public string CreateSession(GameMode gameMode, params string[] userIds)
        {
            var session = new GameSession(gameMode, userIds);
            session.SetOnSessionEndAction(() => OnSessionEnd(session));
            sessions[session.Id] = session;
            return session.Id;
        }

        private void OnSessionEnd(GameSession session)
        {
            if (session.UserIds.Count <= 2)
            {
                using var scope = serviceScopeFactory.CreateScope();
                var ratingService = scope.ServiceProvider.GetService<IRatingService>();
                foreach (var userId in session.UserIds)
                {
                    if (session.GetUserTime(userId) == 0)
                    {
                        ratingService.RemoveDuelRatingForInactivity(userId, session.GameMode);
                    }
                }
            }
            this.DeleteSession(session.Id);
        }

        public void DeleteSession(string sessionId)
        {
            sessions.TryRemove(sessionId, out _);
        }

        public bool TryGetSession(string sessionId, out GameSession gameSession)
        {
            return sessions.TryGetValue(sessionId, out gameSession);
        }
    }
}
