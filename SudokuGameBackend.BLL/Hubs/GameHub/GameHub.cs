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

        public GameHub(IGameSessionsService gameSessionsService, IRatingService ratingService)
        {
            this.gameSessionsService = gameSessionsService;
            this.ratingService = ratingService;
        }

        public async void GameInit(string sessionId)
        {
            if (gameSessionsService.TryGetSession(sessionId, out var session))
            {
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

        public async void UpdateProgress(string sessionId, List<RegularSudokuDto> sudokuDtos)
        {
            if (gameSessionsService.TryGetSession(sessionId, out var session))
            {
                if (session.IsSolved(sudokuDtos))
                {
                    session.SetFinishTime(Context.UserIdentifier, DateTime.Now);
                    session.Mutex.WaitOne();
                    if (!session.HasWinner)
                    {
                        if (session.UserIds.Count == 2)
                        {
                            var winnerId = Context.UserIdentifier;
                            var loserId = session.UserIds.Where(id => id != winnerId).First();
                            var ratings = ratingService.CalculateAndSaveDuelRatings(winnerId, loserId, session.GameMode);
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
                    session.Mutex.ReleaseMutex();
                    ratingService.UpdateSolvingRating(Context.UserIdentifier, gameResult.Time, session.GameMode);
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
            }
        }
    }
}
