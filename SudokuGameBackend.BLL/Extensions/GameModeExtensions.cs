using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.BLL.Extensions
{
    public static class GameModeExtensions
    {
        public static bool IsValid(this GameMode gameMode)
        {
            return Enum.IsDefined(typeof(GameMode), gameMode);
        }
    }
}
