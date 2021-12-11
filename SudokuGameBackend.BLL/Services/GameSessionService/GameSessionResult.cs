using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.BLL.Services
{
    public class GameSessionResult
    {
        public string WinnerId { get; }
        public Dictionary<string, int> NewRatings { get; }

        public GameSessionResult(string winnerId, Dictionary<string, int> newRatings)
        {
            WinnerId = winnerId;
            NewRatings = newRatings;
        }
    }
}
