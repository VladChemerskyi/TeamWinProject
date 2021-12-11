using SudokuStandard;
using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.BLL.Services
{
    class DifficultyMatcher
    {
        public RatingRange EasyRatingRange { get; } = new RatingRange(3500, 4500);
        public RatingRange MediumRatingRange { get; } = new RatingRange(5600, 6600);
        public RatingRange HardRatingRange { get; } = new RatingRange(10650, 11650);

        public RatingRange[] RatingRangesFromGameMode(GameMode gameMode)
        {
            RatingRange[] ratingRanges;
            switch (gameMode)
            {
                case GameMode.OnePuzzleEazy:
                    ratingRanges = new RatingRange[] { EasyRatingRange };
                    break;
                case GameMode.OnePuzzleMedium:
                    ratingRanges = new RatingRange[] { MediumRatingRange };
                    break;
                case GameMode.OnePuzzleHard:
                    ratingRanges = new RatingRange[] { HardRatingRange };
                    break;
                case GameMode.ThreePuzzles:
                    ratingRanges = new RatingRange[] { EasyRatingRange, MediumRatingRange, HardRatingRange };
                    break;
                default:
                    ratingRanges = new RatingRange[] { };
                    break;
            }
            return ratingRanges;
        }
    }
}
