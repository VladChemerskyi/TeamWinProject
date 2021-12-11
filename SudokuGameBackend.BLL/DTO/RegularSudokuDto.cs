using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.BLL.DTO
{
    public class RegularSudokuDto
    {
        public int Id { get; set; }
        public int[] BoardArray { get; set; }
    }
}
