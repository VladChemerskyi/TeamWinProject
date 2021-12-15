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
}
