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

        public override string ToString()
        {
            return $"GetRatingInput(GameMode: {GameMode})";
        }
    }
}
