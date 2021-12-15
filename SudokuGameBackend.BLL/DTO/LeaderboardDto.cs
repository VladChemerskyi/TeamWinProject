using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.BLL.DTO
{
    public class LeaderboardDto
    {
        public List<RatingDto> Ratings { get; set; }
        public RatingDto CurrentPlace { get; set; }
        public int? CalibrationGamesLeft { get; set; }
    }
}
