using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace SudokuStandard
{
    public class RegularSudoku
    {
        [JsonInclude]
        public int[] BoardArray { get; private set; }

        [JsonInclude]
        public int[] SolutionArray { get; private set; }

        [JsonInclude]
        public int Rating { get; private set; }

        public RegularSudoku(int[] boardArray, int[] solutionArray, int rating)
        {
            BoardArray = boardArray;
            SolutionArray = solutionArray;
            Rating = rating;
        }

        public RegularSudoku(int[,] board, int[,] solution, int rating) 
            : this(board.ToArray(), solution.ToArray(), rating) 
        { }

        public static bool IsSolved(int[] boardArray, int[] solutionArray)
        {
            return Enumerable.SequenceEqual(boardArray, solutionArray);
        }
    }
}
