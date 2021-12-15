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
using Microsoft.Extensions.Logging;
using SudokuGameBackend.BLL.Exceptions;

namespace SudokuGameBackend.BLL.Hubs
{
    [Authorize]
    public class GameHub : Hub
    {
        private readonly IGameSessionsService gameSessionsService;
        private readonly IRatingService ratingService;
        private readonly IUserService userService;
        private readonly IStatsService statsService;
        private readonly ILogger<GameHub> logger;

        public GameHub(
            IGameSessionsService gameSessionsService, 
            IRatingService ratingService, 
            IUserService userService,
            IStatsService statsService,
            ILogger<GameHub> logger)
        {
            this.gameSessionsService = gameSessionsService;
            this.ratingService = ratingService;
            this.userService = userService;
            this.statsService = statsService;
            this.logger = logger;
        }

        public async Task GameInit(string sessionId)
        {
            try
            {
                logger.LogDebug($"GameInit. userId: {Context.UserIdentifier}, sessionId: {sessionId}");
                if (string.IsNullOrEmpty(sessionId))
                {
                    throw new GameHubException($"Invalid sessionId value: {sessionId}");
                }
                if (gameSessionsService.TryGetSession(sessionId, out var session))
                {
                    await Clients.Caller.SendAsync("PlayersInfo", await GetPlayersInfo(session));
                    logger.LogTrace($"PlayersInfo sent. userId: {Context.UserIdentifier}, sessionId: {sessionId}");
                    session.SetUserConnectionId(Context.UserIdentifier, Context.ConnectionId);
                    session.SetUserReady(Context.UserIdentifier);
                    if (session.AllUsersReady)
                    {
                        logger.LogDebug($"Session.AllUsersReady. sessionId: {sessionId}");
                        var userIds = session.UserIds;
                        var tasks = new Task[userIds.Count];
                        for (int i = 0; i < userIds.Count; i++)
                        {
                            tasks[i] = Clients.User(userIds[i]).SendAsync("GameStarted", session.GetPuzzlesDto(), session.Duration);
                            logger.LogTrace($"GameStarted sent. userId: {userIds[i]}, sessionId: {sessionId}");
                        }
                        await Task.WhenAll(tasks);
                        session.SetStartTime(DateTime.Now);

                        var gamesCountIncrementMethod = GetGamesCountIncrementMethod(session.UserIds.Count);
                        foreach (var userId in session.UserIds)
                        {
                            await gamesCountIncrementMethod.Invoke(userId, session.GameMode);
                        }
                    }
                }
                else
                {
                    logger.LogWarning($"GameInit. Can't get session. userId: {Context.UserIdentifier}, sessionId: {sessionId}");
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning($"GameInit exception: {ex}");
                await Clients.Caller.SendAsync("Error");
            }
        }

        private Func<string, GameMode, Task> GetGamesCountIncrementMethod(int usersCount)
        {
            Func<string, GameMode, Task> method = statsService.IncrementSingleGamesCount;
            if (usersCount > 1)
            {
                method = statsService.IncrementDuelGamesCount;
            }
            return method;
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
            var userTime = await ratingService.GetUserSolvingRating(userId, gameMode);
            return new UserInfo
            {
                Name = user.Name,
                Rating = userRating,
                Time = userTime
            };
        }

        public async Task UpdateProgress(string sessionId, List<RegularSudokuDto> sudokuDtos)
        {
            try
            {
                logger.LogDebug($"UpdateProgress. userId: {Context.UserIdentifier}, sessionId: {sessionId}");
                if (string.IsNullOrEmpty(sessionId))
                {
                    throw new GameHubException($"Invalid sessionId value: {sessionId}");
                }
                else if (sudokuDtos == null || sudokuDtos.Count == 0 || !sudokuDtos.All(puzzle => puzzle.IsValid))
                {
                    throw new GameHubException($"Invalid puzzles list.");
                }
                if (gameSessionsService.TryGetSession(sessionId, out var session))
                {
                    if (session.IsSolved(sudokuDtos))
                    {
                        session.SetFinishTime(Context.UserIdentifier, DateTime.Now);
                        logger.LogDebug($"User solved puzzle. userId: {Context.UserIdentifier}, sessionId: {sessionId}");
                        session.Semaphore.WaitOne();
                        if (!session.HasResult)
                        {
                            logger.LogDebug($"User won. userId: {Context.UserIdentifier}, sessionId: {sessionId}");
                            await session.CreatePuzzleSolvedResult(Context.UserIdentifier, ratingService, statsService);
                        }
                        session.SetUserCompletionPercent(Context.UserIdentifier, 10000);
                        var gameResult = await session.GetGameResult(Context.UserIdentifier, ratingService);
                        session.Semaphore.Release();
                        logger.LogDebug($"User result. userId: {Context.UserIdentifier}, sessionId: {sessionId}, GameResult: {gameResult}");
                        await Clients.Caller.SendAsync("GameResult", gameResult);
                        logger.LogTrace($"GameResult sent. userId: {Context.UserIdentifier}, sessionId: {sessionId}");
                    }
                    var completionPercent = session.GetCompleteonPercent(sudokuDtos);
                    session.SetUserCompletionPercent(Context.UserIdentifier, completionPercent);
                    if (session.UserIds.Count > 1)
                    {
                        foreach (var userId in session.UserIds)
                        {
                            if (userId != Context.UserIdentifier)
                            {
                                await Clients.User(userId).SendAsync("OpponentCompletionPercent", completionPercent);
                                logger.LogTrace($"OpponentCompletionPercent sent. userId: {userId}, sessionId: {sessionId}");
                            }
                        }
                    }
                    if (session.AllUsersFinished)
                    {
                        gameSessionsService.DeleteSession(session.Id);
                    }
                }
                else
                {
                    logger.LogWarning($"UpdateProgress. Can't get session. userId: {Context.UserIdentifier}, sessionId: {sessionId}");
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning($"UpdateProgress exception: {ex}");
                await Clients.Caller.SendAsync("Error");
            }
        }
    }
}
