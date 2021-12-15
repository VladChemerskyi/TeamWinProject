using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.BLL.Helpers
{
    public enum GameResultType
    {
        Victory = 1,
        Defeat,
        VictoryByCompletionPercent,
        DefeatByCompletionPercent,
        Draw
    }

    public static class GameResultTypeExtension
    {
        public static bool IsVictory(this GameResultType gameResultType)
        {
            return gameResultType == GameResultType.Victory || gameResultType == GameResultType.VictoryByCompletionPercent;
        }
    }
}
