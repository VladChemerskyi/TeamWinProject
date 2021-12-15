using SudokuGameBackend.BLL.DTO;
using SudokuGameBackend.BLL.Exceptions;
using SudokuGameBackend.BLL.Helpers;
using SudokuGameBackend.BLL.Interfaces;
using SudokuStandard;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace SudokuGameBackend.BLL.Services
{
    public class GameSession
    {
        private readonly ConcurrentDictionary<string, UserState> userStates;
        private readonly ConcurrentDictionary<int, RegularSudoku> sudokuPuzzles;
        private GameSessionResult gameResult;
        private System.Timers.Timer sessionTimer;
        private readonly ElapsedEventHandler onSessionAborted;
        private readonly ElapsedEventHandler onSessionEnded;

        public string Id { get; }
        public GameMode GameMode { get; }
        public bool AllUsersReady
        {
            get => userStates.Values.All(state => state.IsReady);
        }
        public List<string> UserIds
        {
            get => userStates.Keys.ToList();
        }
        public DateTime? StartTime { get; private set; }
        public GameSessionResult GameResult
        {
            get => gameResult;
            set
            {
                if (gameResult == null)
                {
                    gameResult = value;
                }
            }
        }
        public bool HasResult { get => GameResult != null; }
        public Semaphore Semaphore { get; }
        public bool AllUsersFinished
        {
            get => userStates.Values.All(state => state.FinishTime.HasValue);
        }

        public GameSession(GameMode gameMode, Func<GameSession, Task> onSessionAborted, Func<GameSession, Task> onSessionEnded, params string[] userIds)
        {
            GameMode = gameMode;
            this.onSessionAborted = async (s, e) => await onSessionAborted.Invoke(this);
            this.onSessionEnded = async (s, e) => await onSessionEnded.Invoke(this);

            userStates = new ConcurrentDictionary<string, UserState>();
            foreach (var userId in userIds)
            {
                userStates[userId] = new UserState();
            }

            Id = Guid.NewGuid().ToString();

            sudokuPuzzles = new ConcurrentDictionary<int, RegularSudoku>();
            var ratingRanges = new DifficultyMatcher().RatingRangesFromGameMode(gameMode);
            var sudokuGenerator = new RegularSudokuGenerator();
            for (int i = 0; i < ratingRanges.Length; ++i)
            {
                var sudoku = sudokuGenerator.Generate(ratingRanges[i]);
                sudokuPuzzles[i] = sudoku;
            }
            Semaphore = new Semaphore(1, 1);

            sessionTimer = new System.Timers.Timer
            {
                // Ten seconds for users to connect.
                Interval = 10000,
                AutoReset = false
            };
            sessionTimer.Elapsed += this.onSessionAborted;
            sessionTimer.Start();
        }

        public void SetUserReady(string userId)
        {
            userStates[userId].IsReady = true;
            if (AllUsersReady)
            {
                sessionTimer.Stop();
                sessionTimer.Elapsed -= onSessionAborted;
            }
        }

        public void SetStartTime(DateTime startTime)
        {
            StartTime = startTime;
            // 10 minutes for game session.
            sessionTimer.Interval = 600000;
            sessionTimer.Elapsed += onSessionEnded;
            sessionTimer.Start();
        }

        public void SetFinishTime(string userId, DateTime finishTime)
        {
            userStates[userId].FinishTime = finishTime;
        }

        public List<RegularSudokuDto> GetPuzzlesDto()
        {
            var puzzles = new List<RegularSudokuDto>();
            foreach (var pair in sudokuPuzzles)
            {
                puzzles.Add(new RegularSudokuDto
                {
                    Id = pair.Key,
                    BoardArray = pair.Value.BoardArray
                });
            }
            return puzzles;
        }

        public int GetCompleteonPercent(List<RegularSudokuDto> puzzles)
        {
            int totalEmpty = sudokuPuzzles.Values.Sum(puzzle => puzzle.BoardArray.Count(value => value == 0));
            int currentCorrectlyFilled = puzzles.Sum(puzzle => GetCorrectlyFilledCount(puzzle));
            int completionPercent = (int)Math.Round((double)currentCorrectlyFilled / totalEmpty * 10000);
            return completionPercent;
        }

        public void SetUserCompletionPercent(string userId, int completionPercent)
        {
            userStates[userId].CompletionPercent = completionPercent;
        }

        public bool IsSolved(List<RegularSudokuDto> puzzles)
        {
            return puzzles.All(puzzle => RegularSudoku.IsSolved(puzzle.BoardArray, sudokuPuzzles[puzzle.Id].SolutionArray));
        }

        public int GetUserTime(string userId)
        {
            int userTime = -1;
            var finishTime = userStates[userId].FinishTime;
            if (finishTime.HasValue && StartTime.HasValue)
            {
                userTime = (int)(finishTime.Value - StartTime.Value).TotalMilliseconds;
            }
            return userTime;
        }

        public void SetUserConnectionId(string userId, string connectionId)
        {
            userStates[userId].ConnectionId = connectionId;
        }

        public string GetUserConnectionId(string userId)
        {
            return userStates[userId].ConnectionId;
        }

        public async Task CreatePuzzleSolvedResult(string winnerId, IRatingService ratingService, IStatsService statsService)
        {
            if (HasResult)
            {
                throw new GameSessionException("Session already has result.");
            }

            if (UserIds.Count == 2)
            {
                var loserId = UserIds.Where(id => id != winnerId).First();
                var ratings = await ratingService.UpdateUsersRatings(
                    winnerId, GameResultType.Victory, loserId, GameResultType.Defeat, GameMode);
                GameResult = new GameSessionResult(new Dictionary<string, GameResultType> 
                {
                    { winnerId, GameResultType.Victory },
                    { loserId, GameResultType.Defeat },
                }, 
                ratings.NewRatings, ratings.OldRatings);
                await statsService.IncrementDuelWinsCount(winnerId, GameMode);
            }
            else if (UserIds.Count == 1)
            {
                GameResult = new GameSessionResult(new Dictionary<string, GameResultType> 
                {
                    { winnerId, GameResultType.Victory }
                }, 
                null, null);
            }
        }

        public async Task CreateTimeExpiredResult(IRatingService ratingService)
        {
            if (HasResult)
            {
                throw new GameSessionException("Session already has a winner.");
            }

            if (userStates.Count == 2)
            {
                var user1State = userStates.ElementAt(0);
                var user2State = userStates.ElementAt(1);
                if (user1State.Value.CompletionPercent == user2State.Value.CompletionPercent)
                {
                    var ratings = await ratingService.UpdateUsersRatings(
                        user1State.Key, GameResultType.Draw, user2State.Key, GameResultType.Draw, GameMode);
                    GameResult = new GameSessionResult(new Dictionary<string, GameResultType>
                    {
                        { user1State.Key, GameResultType.Draw },
                        { user2State.Key, GameResultType.Draw },
                    }, 
                    ratings.NewRatings, ratings.OldRatings);
                }
                else
                {
                    var winnerId = user1State.Key;
                    var loserId = user2State.Key;
                    if (user2State.Value.CompletionPercent > user1State.Value.CompletionPercent)
                    {
                        winnerId = user2State.Key;
                        loserId = user1State.Key;
                    }
                    var ratings = await ratingService.UpdateUsersRatings(
                        winnerId, 
                        GameResultType.VictoryByCompletionPercent,
                        loserId, 
                        GameResultType.DefeatByCompletionPercent,
                        GameMode);
                    GameResult = new GameSessionResult(new Dictionary<string, GameResultType>
                    {
                        { winnerId, GameResultType.VictoryByCompletionPercent },
                        { loserId, GameResultType.DefeatByCompletionPercent },
                    }, 
                    ratings.NewRatings, ratings.OldRatings);
                }
            }
            else if (UserIds.Count == 1)
            {
                GameResult = new GameSessionResult(new Dictionary<string, GameResultType>
                {
                    { userStates.Keys.First(), GameResultType.Defeat }
                },
                null, null);
            }
        }

        public async Task<GameResultDto> GetGameResult(string userId, IRatingService ratingService)
        {
            if (!HasResult)
            {
                throw new GameSessionException($"Session has no result. sessionId: {Id}");
            }
            if (userStates[userId].ResultWasCreated)
            {
                throw new GameSessionException($"A result has already been created for this user. userId: {userId}, sessionId: {Id}");
            }

            var userTime = GetUserTime(userId);
            var isBestNewTime = userTime != -1 && await ratingService.UpdateSolvingRating(userId, userTime, GameMode);
            var result = new GameResultDto
            {
                GameResultType = GameResult.UsersGameResults[userId],
                NewDuelRating = GameResult.NewRatings?[userId] ?? -1 ,
                OldDuelRating = GameResult.OldRatings?[userId] ?? -1,
                Time = userTime,
                IsNewBestTime = isBestNewTime,
                BestTime = isBestNewTime ? userTime : await ratingService.GetUserSolvingRating(userId, GameMode),
                GameMode = GameMode,
                CompletionPercent = userStates[userId].CompletionPercent
            };
            userStates[userId].ResultWasCreated = true;
            return result;
        }

        public bool UserResultWasCreated(string userId)
        {
            return userStates[userId].ResultWasCreated;
        }

        private int GetCorrectlyFilledCount(RegularSudokuDto sudokuDto)
        {
            var puzzle = sudokuPuzzles[sudokuDto.Id];
            int correctlyFilled = 0;
            for (int i = 0; i < puzzle.BoardArray.Length; ++i)
            {
                if (puzzle.BoardArray[i] == 0 && puzzle.SolutionArray[i] == sudokuDto.BoardArray[i])
                {
                    correctlyFilled++;
                }
            }
            return correctlyFilled;
        }

        public void Dispose()
        {
            sessionTimer.Close();
        }
    }
}
