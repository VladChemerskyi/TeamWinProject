using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SudokuGameBackend.BLL.InputModels
{
    public class GetLeaderboardInput
    {
        [Required]
        public GameMode? GameMode { get; set; }
        public int Index { get; set; } = 0;
        public int Count { get; set; } = 20;

        public override string ToString()
        {
            return $"GetRatingInput(GameMode: {GameMode}, Index: {Index}, Count: {Count})";
        }
    }
}
