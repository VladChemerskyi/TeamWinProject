using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.DAL.Entities
{
    public class SingleStats
    {
        public string UserId { get; set; }
        public GameMode GameMode { get; set; }
        public int GamesStarted { get; set; }

        public User User { get; set; }
    }
}
