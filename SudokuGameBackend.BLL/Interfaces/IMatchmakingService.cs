using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.BLL.Interfaces
{
    public interface IMatchmakingService
    {
        bool TryFindOpponent(GameMode gameMode, int rating, out string opponentId);
        void AddToQueue(string userId, int rating, GameMode gameMode);
    }
}
