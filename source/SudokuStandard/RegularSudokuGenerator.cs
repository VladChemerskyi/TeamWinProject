using System;
using System.Collections.Generic;
using System.Text;
using Kermalis.SudokuSolver.Core;
using System.Threading.Tasks;
using System.Linq;
using org.mariuszgromada.math.janetsudoku;

namespace SudokuStandard
{
    public struct Cell
    {
        public int Row { get; private set; }
        public int Col { get; private set; }
        public int Index { get; private set; }

        public Cell(int row, int col)
        {
            Row = row;
            Col = col;
            Index = row * 9 + col;
        }

        public Cell(int index)
        {
            Row = index / 9;
            Col = index % 9;
            Index = index;
        }
    }

    public class RegularSudokuGenerator
    {
        private readonly Random rand = new Random();
        private readonly IEnumerable<int> possibleCellCandidates = Enumerable.Range(1, 9);

        public RegularSudoku Generate(int minRating, int maxRating)
        {
            var ratingRange = new RatingRange(minRating, maxRating);
            return Generate(ratingRange);
        }

        public async Task<RegularSudoku> GenerateAsync(int minRating, int maxRating)
        {
            return await Task.Run(() => Generate(minRating, maxRating));
        }

        public RegularSudoku Generate(DifficultyLevel difficultyLevel)
        {
            var ratingRange = RatingRange.FromDifficultyLevel(difficultyLevel);
            return Generate(ratingRange);
        }

        public async Task<RegularSudoku> GenerateAsync(DifficultyLevel difficultyLevel)
        {
            return await Task.Run(() => Generate(difficultyLevel));
        }

        public async Task<RegularSudoku> GenerateAsync(RatingRange ratingRange)
        {
            return await Task.Run(() => Generate(ratingRange));
        }

        public RegularSudoku Generate(RatingRange ratingRange)
        {
            RegularSudoku result;
            do
            {
                result = TryGenerate(ratingRange);
            }
            while (result == null);
            return result;
        }       

        private RegularSudoku TryGenerate(RatingRange ratingRange)
        {
            int rating = 0;
            int iterations = 0;
            int itersWithoutRating = 35;
            int maxIterations = 60;
            var availableCells = Enumerable.Range(0, 81).ToList();
            var board = GenerateRandomBoard();
            while (!ratingRange.Matches(rating))
            {
                if (iterations >= maxIterations)
                {
                    return null;
                }

                iterations++;

                if (availableCells.Count > 0)
                {
                    var randomCell = new Cell(availableCells[rand.Next(availableCells.Count)]);
                    var erasedValue = EraseCell(board, randomCell);
                    availableCells.Remove(randomCell.Index);

                    var solver = new SudokuSolver(board);
                    int solutionsNumber = solver.findAllSolutions();
                    if (solutionsNumber == 1)
                    {
                        if (iterations > itersWithoutRating)
                        {
                            rating = CalculateRating(board);
                            if (rating == 0)
                            {
                                return null;
                            }
                        }
                    }
                    else
                    {
                        UndoErasing(board, randomCell, erasedValue);
                    }
                }
                else
                {
                    return null;
                }

            }
            return new RegularSudoku(board, rating);
        }

        private int EraseCell(int[,] board, Cell cell)
        {
            var value = board[cell.Row, cell.Col];
            board[cell.Row, cell.Col] = 0;
            return value;
        }

        private void UndoErasing(int[,] board, Cell cell, int value)
        {
            board[cell.Row, cell.Col] = value;
        }

        private int CalculateRating(int[,] sudokuValues)
        {
            int rating = 0;
            var puzzle = new Puzzle(sudokuValues);
            var solver = new Solver(puzzle);
            if (solver.Solve(out var solvingMethods))
            {
                var checkedMethods = new HashSet<SolvingMethod>();
                foreach (var solvingMethod in solvingMethods)
                {
                    var methodCost = MethodCost.FromSolvingMethod(solvingMethod);
                    if (!checkedMethods.Contains(solvingMethod))
                    {
                        rating += methodCost.FirstUseCost;
                    }
                    else
                    {
                        rating += methodCost.SubsequentUsesCost;
                    }
                    checkedMethods.Add(solvingMethod);
                }
            }
            return rating;
        }

        public int[,] GenerateRandomBoard()
        {
            int[,] board = new int[9, 9];
            FillNext(board, 0);
            return board;
        }

        private bool FillNext(int[,] board, int cellIndex)
        {
            if (cellIndex == 81)
            {
                return true;
            }

            GetRowCol(cellIndex, out int row, out int col);
            var candidates = GetCandidates(board, row, col);
            do
            {
                if (candidates.Count() > 0)
                {
                    var randCandidate = candidates[rand.Next(candidates.Count)];
                    candidates.Remove(randCandidate);
                    board[row, col] = randCandidate;
                }
                else
                {
                    board[row, col] = 0;
                    return false;
                }
            }
            while (!FillNext(board, cellIndex + 1));
            return true;
        }

        private List<int> GetCandidates(int[,] board, int row, int col)
        {
            var rowNumbers = GetRowNumbers(board, row);
            var colNumbers = GetColNumbers(board, col);
            var rectNumbers = GetRectNumbers(board, row, col);

            var filledNumbers = new HashSet<int>(rowNumbers.Concat(colNumbers).Concat(rectNumbers));
            var candidates = possibleCellCandidates.Except(filledNumbers);

            return candidates.ToList();
        }

        private List<int> GetRowNumbers(int[,] board, int row)
        {
            var numbers = new List<int>(9);
            for (int i = 0; i < 9; ++i)
            {
                if (board[row, i] != 0)
                {
                    numbers.Add(board[row, i]);
                }
            }
            return numbers;
        }

        private List<int> GetColNumbers(int[,] board, int col)
        {
            var numbers = new List<int>(9);
            for (int i = 0; i < 9; ++i)
            {
                if (board[i, col] != 0)
                {
                    numbers.Add(board[i, col]);
                }
            }
            return numbers;
        }

        private List<int> GetRectNumbers(int[,] board, int row, int col)
        {
            var numbers = new List<int>(9);
            int rectTopLeftRow = row / 3 * 3;
            int rectTopLeftCol = col / 3 * 3;
            for (int i = 0; i < 3; ++i)
            {
                for (int k = 0; k < 3; ++k)
                {
                    int currentRow = rectTopLeftRow + i;
                    int currentCol = rectTopLeftCol + k;
                    if (board[currentRow, currentCol] != 0)
                    {
                        numbers.Add(board[currentRow, currentCol]);
                    }
                }
            }
            return numbers;
        }

        private void GetRowCol(int cellIndex, out int row, out int col)
        {
            row = cellIndex / 9;
            col = cellIndex % 9;
        }
    }
}
