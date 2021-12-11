using Microsoft.AspNetCore.SignalR;
using SudokuGameBackend.BLL.DTO;
using SudokuGameBackend.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using SudokuGameBackend.BLL.Services;
using Microsoft.AspNetCore.Authorization;

namespace SudokuGameBackend.BLL.Hubs
{
    [Authorize]
    public class GameHub : Hub
    {
        private readonly IGameSessionsService gameSessionsService;
        private readonly IRatingService ratingService;
        private readonly IUserService userService;

        public GameHub(IGameSessionsService gameSessionsService, IRatingService ratingService, IUserService userService)
        {
            this.gameSessionsService = gameSessionsService;
            this.ratingService = ratingService;
            this.userService = userService;
        }

        public async Task GameInit(string sessionId)
        {
            if (gameSessionsService.TryGetSession(sessionId, out var session))
            {
                await Clients.Caller.SendAsync("PlayersInfo", await GetPlayersInfo(session));
                session.SetUserConnectionId(Context.UserIdentifier, Context.ConnectionId);
                session.SetUserReady(Context.UserIdentifier);
                if (session.AllUsersReady)
                {
                    var userIds = session.UserIds;
                    var tasks = new Task[userIds.Count];
                    for (int i = 0; i < userIds.Count; i++)
                    {
                        tasks[i] = Clients.User(userIds[i]).SendAsync("GameStarted", session.GetPuzzlesDto());
                    }
                    await Task.WhenAll(tasks);
                    session.SetStartTime(DateTime.Now);
                }
            }
        }

        private async Task<List<UserInfo>> GetPlayersInfo(GameSession session)
        {
            var playerInfo = await GetUserInfo(Context.UserIdentifier, session.GameMode);
            var usersInfo = new List<UserInfo>(session.UserIds.Count) { playerInfo };
            foreach (var userId in session.UserIds)
            {
                if (userId != Context.UserIdentifier)
                {
                    usersInfo.Add(await GetUserInfo(userId, session.GameMode));
                }
            }
            return usersInfo;
        }

        private async Task<UserInfo> GetUserInfo(string userId, GameMode gameMode)
        {
            var user = await userService.GetUser(userId);
            var userRating = await ratingService.GetUserRating(userId, gameMode);
            return new UserInfo
            {
                Name = user.Name,
                Rating = userRating
            };
        }

        public async Task UpdateProgress(string sessionId, List<RegularSudokuDto> sudokuDtos)
        {
            if (gameSessionsService.TryGetSession(sessionId, out var session))
            {
                if (session.IsSolved(sudokuDtos))
                {
                    session.SetFinishTime(Context.UserIdentifier, DateTime.Now);
                    session.Semaphore.WaitOne();
                    if (!session.HasWinner)
                    {
                        if (session.UserIds.Count == 2)
                        {
                            var winnerId = Context.UserIdentifier;
                            var loserId = session.UserIds.Where(id => id != winnerId).First();
                            var ratings = await ratingService.CalculateDuelRatings(winnerId, loserId, session.GameMode);
                            foreach (var pair in ratings)
                            {
                                await ratingService.UpdateDuelRating(pair.Key, session.GameMode, pair.Value);
                            }
                            session.GameResult = new GameSessionResult(winnerId, ratings);
                        }
                        else if (session.UserIds.Count == 1)
                        {
                            var newRatings = new Dictionary<string, int>
                            {
                                { Context.UserIdentifier, -1 }
                            };
                            session.GameResult = new GameSessionResult(Context.UserIdentifier, newRatings);
                        }
                    }
                    var gameResult = new GameResult
                    {
                        IsVictory = session.GameResult.WinnerId == Context.UserIdentifier,
                        NewDuelRating = session.GameResult.NewRatings[Context.UserIdentifier],
                        Time = session.GetUserTime(Context.UserIdentifier)
                    };
                    session.Semaphore.Release();
                    await ratingService.UpdateSolvingRating(Context.UserIdentifier, gameResult.Time, session.GameMode);
                    await Clients.Caller.SendAsync("GameResult", gameResult);
                }
                var completionPercent = session.GetCompleteonPercent(sudokuDtos);
                foreach (var userId in session.UserIds)
                {
                    if (userId != Context.UserIdentifier)
                    {
                        await Clients.User(userId).SendAsync("OpponentCompletionPercent", completionPercent);
                    }
                }
                if (session.AllUsersFinished)
                {
                    gameSessionsService.DeleteSession(session.Id);
                }
            }
        }
    }
}
