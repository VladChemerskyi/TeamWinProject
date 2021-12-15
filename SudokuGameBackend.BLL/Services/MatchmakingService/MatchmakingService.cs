using Microsoft.Extensions.Logging;
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
        private readonly ILogger<MatchmakingService> logger;

        public MatchmakingService(ILogger<MatchmakingService> logger)
        {
            queue = new List<QueueItem>();
            mutex = new Mutex();
            this.logger = logger;
        }

        public void AddToQueue(string userId, int rating, GameMode gameMode)
        {
            mutex.WaitOne();
            queue.Add(new QueueItem(userId, rating, gameMode));
            mutex.ReleaseMutex();
            logger.LogDebug($"Added to queue. userId: {userId}, rating: {rating}, gameMode: {gameMode}");
        }

        public void RemoveFromQueue(string userId)
        {
            mutex.WaitOne();
            int nRemoved = queue.RemoveAll(item => item.UserId == userId);
            mutex.ReleaseMutex();
            logger.LogDebug($"Removed from queue. userId: {userId}, nRemoved: {nRemoved}");
        }

        public bool TryFindOpponent(GameMode gameMode, int rating, string userId, out string opponentId)
        {
            logger.LogDebug($"TryFindOpponent. userId: {userId}, rating: {rating}, gameMode: {gameMode}");
            bool opponentFound = false;
            opponentId = null;
            mutex.WaitOne();
            var matchedItem = queue.FirstOrDefault(item => userId != item.UserId && item.Match(gameMode, rating));
            if (matchedItem != null)
            {
                queue.Remove(matchedItem);
                opponentId = matchedItem.UserId;
                opponentFound = true;
                logger.LogDebug($"TryFindOpponent success. userId: {userId}, opponentId: {opponentId}");
            }
            mutex.ReleaseMutex();
            return opponentFound;
        }
    }
}
