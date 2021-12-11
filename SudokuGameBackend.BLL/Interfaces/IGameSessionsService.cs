using SudokuGameBackend.BLL.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.BLL.Interfaces
{
    public interface IGameSessionsService
    {
        string CreateSession(GameMode gameMode, params string[] userIds);
        bool TryGetSession(string sessionId, out GameSession gameSession);
        void DeleteSession(string sessionId);
    }
}
