using SudokuGameBackend.BLL.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.BLL.Services
{
    public class GameSessionResult
    {
        public Dictionary<string, GameResultType> UsersGameResults { get; }
        public Dictionary<string, int> NewRatings { get; }
        public Dictionary<string, int> OldRatings { get; }

        public GameSessionResult(
            Dictionary<string, GameResultType> userGameResults, 
            Dictionary<string, int> newRatings, 
            Dictionary<string, int> oldRatings)
        {
            UsersGameResults = userGameResults;
            NewRatings = newRatings;
            OldRatings = oldRatings;
        }
    }
}
