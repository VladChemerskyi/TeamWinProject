using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.BLL.DTO
{
    public class UserStatsDto
    {
        public Dictionary<int, int> DuelStats { get; set; }
        public Dictionary<int, int> SolvingStats { get; set; }
    }
}
