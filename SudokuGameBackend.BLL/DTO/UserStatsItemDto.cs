using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.BLL.DTO
{
    public class UserStatsItemDto
    {
        public int? SolvingTime { get; set; }
        public int? DuelRating { get; set; }
        public int TotalGamesStarted { get; set; }
        public int SingleGamesStarted { get; set; }
        public int DuelGamesStarted { get; set; }
        public int DuelGamesWon { get; set; }
        public int? DuelGameWinsPercent { get; set; }
    }
}
