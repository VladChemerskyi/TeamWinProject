using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.BLL.DTO
{
    public class RegularSolvedSudokuDto
    {
        public int Id { get; set; }
        public int[] BoardArray { get; set; }
        public int[] SolutionArray { get; set; }
    }
}
