using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.BLL.Services
{
    class QueueItem
    {
        private const int maxRatingDelta = 300;
        public string UserId { get; }
        public int Rating { get; }
        public GameMode GameMode { get; }

        public QueueItem(string userId, int rating, GameMode gameMode)
        {
            UserId = userId;
            Rating = rating;
            GameMode = gameMode;
        }

        public bool Match(GameMode gameMode, int rating)
        {
            return GameMode == gameMode && Math.Abs(Rating - rating) <= maxRatingDelta;
        }
    }
}
