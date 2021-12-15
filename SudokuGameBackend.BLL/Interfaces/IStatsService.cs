using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SudokuGameBackend.BLL.Interfaces
{
    public interface IStatsService
    {
        Task SetInitialStats(string userId);
        Task IncrementDuelGamesCount(string userId, GameMode gameMode);
        Task IncrementSingleGamesCount(string userId, GameMode gameMode);
        Task IncrementDuelWinsCount(string userId, GameMode gameMode);
    }
}
