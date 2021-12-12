using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.BLL.Hubs
{
    class GameResult
    {
        public bool IsVictory { get; set; }
        public int Time { get; set; }
        public int NewDuelRating { get; set; }
        public bool IsNewBestTime { get; set; }
    }
}
