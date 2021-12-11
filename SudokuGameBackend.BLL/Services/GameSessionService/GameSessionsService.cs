using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using SudokuGameBackend.BLL.Hubs;
using SudokuGameBackend.BLL.Interfaces;
using System.Collections.Concurrent;
using System.Threading.Tasks;

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

        // TODO: Make things async.
        public string CreateSession(GameMode gameMode, params string[] userIds)
        {
            var session = new GameSession(gameMode, userIds);
            session.SetOnSessionEndAction(async () => await OnSessionEnd(session));
            sessions[session.Id] = session;
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
