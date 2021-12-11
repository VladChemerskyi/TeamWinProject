﻿using SudokuGameBackend.BLL.Interfaces;
using SudokuStandard;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuGameBackend.BLL.Services
{
    public class GameSessionsService : IGameSessionsService
    {
        private readonly ConcurrentDictionary<string, GameSession> sessions;

        public GameSessionsService()
        {
            sessions = new ConcurrentDictionary<string, GameSession>();
        }

        public string CreateSession(GameMode gameMode, params string[] userIds)
        {
            var session = new GameSession(gameMode, userIds);
            sessions[session.Id] = session;
            return session.Id;
        }

        public void DeleteSession(string sessionId)
        {
            sessions.TryRemove(sessionId, out _);
        }

        public bool TryGetSession(string sessionId, out GameSession gameSession)
        {
            return sessions.TryGetValue(sessionId, out gameSession);
        }
    }
}
