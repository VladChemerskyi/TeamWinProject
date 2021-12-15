using SudokuGameBackend.BLL.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.BLL.DTO
{
    public class GameResultDto
    {
        public GameResultType GameResultType { get; set; }
        public int Time { get; set; }
        public int BestTime { get; set; }
        public int NewDuelRating { get; set; }
        public int OldDuelRating { get; set; }
        public bool IsNewBestTime { get; set; }
        public GameMode GameMode { get; set; }
        public int CompletionPercent { get; set; }

        public override string ToString()
        {
            return $"GameResult(GameResultType: {GameResultType}, Time: {Time}, BestTime: {BestTime}, " +
                $"NewDuelRating: {NewDuelRating}, OldDuelRating: {OldDuelRating}, " +
                $"IsNewBestTime: {IsNewBestTime}, GameMode: {GameMode}, CompletionPercent: {CompletionPercent})";
        }
    }
}
