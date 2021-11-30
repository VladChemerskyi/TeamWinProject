using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SudokuStandard
{
    public class RegularSudoku
    {
        [JsonIgnore]
        public int[,] Board { get; private set; }

        [JsonInclude]
        public int[] BoardArray { get; private set; }

        [JsonInclude]
        public int Rating { get; private set; }

        public RegularSudoku(int[,] board, int rating)
        {
            Board = board;
            BoardArray = board.ToArray();
            Rating = rating;
        }
    }
}
