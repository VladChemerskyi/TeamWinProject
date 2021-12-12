using SudokuGameBackend.BLL.DTO;
using SudokuGameBackend.BLL.Helpers;
using SudokuStandard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using SudokuGameBackend.BLL.Interfaces;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace SudokuGameBackend.BLL.Services
{
    public class PuzzleService : IPuzzleService
    {
        private readonly ILogger<PuzzleService> logger;

        public PuzzleService(ILogger<PuzzleService> logger)
        {
            this.logger = logger;
        }

        public async Task<List<RegularSolvedSudokuDto>> GetRegularPuzzle(GameMode gameMode)
        {
            logger.LogDebug($"GetRegularPuzzle. gameMode: {gameMode}");
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var ratingRanges = new DifficultyMatcher().RatingRangesFromGameMode(gameMode);
            var sudokuGenerator = new RegularSudokuGenerator();
            var tasks = ratingRanges.Select(ratingRange =>
            {
                return sudokuGenerator.GenerateAsync(ratingRange);
            });
            var puzzles = await Task.WhenAll(tasks);
            stopWatch.Stop();
            logger.LogDebug($"GetRegularPuzzle success. gameMode: {gameMode}, " +
                $"creationTime: {Math.Round(stopWatch.Elapsed.TotalSeconds, 2)}s");
            return puzzles.Select((puzzle, index) =>
            {
                return new RegularSolvedSudokuDto
                {
                    Id = index,
                    BoardArray = puzzle.BoardArray,
                    SolutionArray = puzzle.SolutionArray
                };
            }).ToList();
        }
    }
}
