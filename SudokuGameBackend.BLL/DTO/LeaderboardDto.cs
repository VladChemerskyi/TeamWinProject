using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SudokuGameBackend.BLL.DTO
{
    public class LeaderboardDto
    {
        [JsonPropertyName("ratings")]
        public List<RatingDto> Ratings { get; set; }

        [JsonPropertyName("currentPlace")]
        public RatingDto CurrentPlace { get; set; }

        [JsonPropertyName("calibrationGamesLeft")]
        public int? CalibrationGamesLeft { get; set; }
    }
}
