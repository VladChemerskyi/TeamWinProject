using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.BLL.Interfaces
{
    public interface IRatingService
    {
        Dictionary<string, int> CalculateAndSaveDuelRatings(string winnerId, string loserId, GameMode gameMode);
        void UpdateSolvingRating(string userId, int time, GameMode gameMode);
        void SetInitialDuelRating(string userId);
    }
}
