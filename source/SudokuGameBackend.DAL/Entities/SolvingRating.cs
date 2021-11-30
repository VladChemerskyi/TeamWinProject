using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.DAL.Entities
{
    public class SolvingRating
    {
        public string UserId { get; set; }
        public GameMode GameMode { get; set; }
        public int Time { get; set; }

        public User User { get; set; }
    }
}
