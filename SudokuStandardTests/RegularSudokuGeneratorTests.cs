using Microsoft.VisualStudio.TestTools.UnitTesting;
using SudokuStandard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.mariuszgromada.math.janetsudoku;
using System.Threading.Tasks;
using System.Diagnostics;
using Kermalis.SudokuSolver.Core;

namespace SudokuStandard.Tests
{
    [TestClass()]
    public class RegularSudokuGeneratorTests
    {
        [TestMethod()]
        public void GenerateRandomBoardTest()
        {
            var generator = new RegularSudokuGenerator();
            var board = generator.GenerateRandomBoard();
            Assert.IsTrue(IsBoardValid(board));
        }

        private bool IsBoardValid(int[,] board)
        {
            for (int i = 0; i < 9; ++i)
            {
                if (!(IsRowValid(board, i) && IsColValid(board, i) && IsRectValid(board, i)))
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsRowValid(int[,] board, int row)
        {
            var numbers = new HashSet<int>();
            for (int i = 0; i < 9; ++i)
            {
                numbers.Add(board[row, i]);
            }
            return IsSetValid(numbers);
        }

        private bool IsColValid(int[,] board, int col)
        {
            var numbers = new HashSet<int>();
            for (int i = 0; i < 9; ++i)
            {
                numbers.Add(board[i, col]);
            }
            return IsSetValid(numbers);
        }

        private bool IsRectValid(int[,] board, int rectIndex)
        {
            var numbers = new HashSet<int>();
            int rectTopLeftRow = rectIndex / 3 * 3;
            int rectTopLeftCol = rectIndex % 3 * 3;
            for (int i = 0; i < 3; ++i)
            {
                for (int k = 0; k < 3; ++k)
                {
                    numbers.Add(board[rectTopLeftRow + i, rectTopLeftCol + k]);
                }
            }
            return IsSetValid(numbers);
        }

        private bool IsSetValid(HashSet<int> set)
        {
            return set.Max() == 9 && set.Min() == 1 && set.Count == 9;
        }

        [TestMethod()]
        public void GenerateTest()
        {
            var ratingRange = RatingRange.FromDifficultyLevel(DifficultyLevel.Diabolical);
            var generator = new RegularSudokuGenerator();
            var sudoku = generator.Generate(ratingRange);
            Assert.IsTrue(ratingRange.Matches(sudoku.Rating));
            Assert.IsTrue(IsSolutionMatchBoard(sudoku.SolutionArray, sudoku.BoardArray));
        }

        private bool IsSolutionMatchBoard(int[] solution, int[] board)
        {
            if (solution.Length != board.Length)
            {
                return false;
            }
            var solver = new Solver(new Puzzle(Get2DBoardArray(board)));
            solver.Solve(out _);
            var currentSolution = solver.Puzzle.GetBoardArray();
            return Enumerable.SequenceEqual(solution, currentSolution);
        }

        private int[,] Get2DBoardArray(int[] board)
        {
            int[,] board2D = new int[9, 9];
            for (int i = 0; i < board.Length; ++i)
            {
                board2D[i / 9, i % 9] = board[i];
            }
            return board2D;
        }

        private double CalculateAverage(Action func, int iters)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < iters; ++i)
            {
                func();
            }
            stopwatch.Stop();
            return stopwatch.Elapsed.TotalMilliseconds / iters;
        }

        [TestMethod()]
        public void AverageGenerateTime()
        {
            int iters = 50;
            var ratingRange = RatingRange.FromDifficultyLevel(DifficultyLevel.Diabolical);
            var generator = new RegularSudokuGenerator();
            var average = CalculateAverage(() => generator.Generate(ratingRange), iters);
            Console.WriteLine($"Average: {(int)average}.");
        }

        [TestMethod()]
        public void AverageGenerateRandomBoardTime()
        {
            int iters = 100;
            var generator = new RegularSudokuGenerator();
            var average = CalculateAverage(() => generator.GenerateRandomBoard(), iters);
            Console.WriteLine($"Average: {average}.");
        }
    }
}
