using SudokuGameBackend.BLL.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SudokuGameBackend.BLL.Services
{
    public class MatchmakingService : IMatchmakingService
    {
        private readonly List<QueueItem> queue;
        private readonly Mutex mutex;

        public MatchmakingService()
        {
            queue = new List<QueueItem>();
            mutex = new Mutex();
        }

        public void AddToQueue(string userId, int rating, GameMode gameMode)
        {
            mutex.WaitOne();
            queue.Add(new QueueItem(userId, rating, gameMode));
            mutex.ReleaseMutex();
        }

        public void RemoveFromQueue(string userId)
        {
            mutex.WaitOne();
            queue.RemoveAll(item => item.UserId == userId);
            mutex.ReleaseMutex();
        }

        public bool TryFindOpponent(GameMode gameMode, int rating, string userId, out string opponentId)
        {
            bool opponentFound = false;
            opponentId = null;
            mutex.WaitOne();
            var mathcedItem = queue.FirstOrDefault(item => userId != item.UserId && item.Match(gameMode, rating));
            if (mathcedItem != null)
            {
                queue.Remove(mathcedItem);
                opponentId = mathcedItem.UserId;
                opponentFound = true;
            }
            mutex.ReleaseMutex();
            return opponentFound;
        }
    }
}
