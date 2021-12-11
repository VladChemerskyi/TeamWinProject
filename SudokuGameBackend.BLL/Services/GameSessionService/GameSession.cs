﻿using SudokuGameBackend.BLL.DTO;
using SudokuStandard;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SudokuGameBackend.BLL.Services
{
    public class GameSession
    {
        private readonly ConcurrentDictionary<string, UserState> userStates;
        private readonly ConcurrentDictionary<int, RegularSudoku> sudokuPuzzles;
        private GameSessionResult gameResult;

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
        public DateTime StartTime { get; private set; }
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
        public bool HasWinner { get => GameResult != null; }
        public Mutex Mutex { get; }


        public GameSession(GameMode gameMode, params string[] userIds)
        {
            GameMode = gameMode;

            userStates = new ConcurrentDictionary<string, UserState>();
            foreach (var userId in userIds)
            {
                userStates[userId] = new UserState();
            }

            Id = Guid.NewGuid().ToString();

            sudokuPuzzles = new ConcurrentDictionary<int, RegularSudoku>();
            var ratingRanges = DifficultyMatcher.RatingRangesFromGameMode(gameMode);
            var sudokuGenerator = new RegularSudokuGenerator();
            for (int i = 0; i < ratingRanges.Length; ++i)
            {
                var sudoku = sudokuGenerator.Generate(ratingRanges[i]);
                sudokuPuzzles[i] = sudoku;
            }
            Mutex = new Mutex();
        }

        public void SetUserReady(string userId)
        {
            userStates[userId].IsReady = true;
        }

        public void SetStartTime(DateTime startTime)
        {
            StartTime = startTime;
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
            int totalEmpty = sudokuPuzzles.Sum(pair => pair.Value.BoardArray.Count(value => value == 0));
            int currentCorrectlyFilled = puzzles.Sum(puzzle => GetCorrectlyFilledCount(puzzle));
            int completionPercent = (int)Math.Round((double)currentCorrectlyFilled / totalEmpty * 100);
            return completionPercent;
        }

        public bool IsSolved(List<RegularSudokuDto> puzzles)
        {
            return puzzles.All(puzzle => RegularSudoku.IsSolved(puzzle.BoardArray, sudokuPuzzles[puzzle.Id].SolutionArray));
        }

        public int GetUserTime(string userId)
        {
            int userTime = 0;
            var finishTime = userStates[userId].FinishTime;
            if (finishTime.HasValue)
            {
                userTime = (int)(finishTime.Value - StartTime).TotalMilliseconds;
            }
            return userTime;
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
    }
}