using SudokuStandard;
using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.BLL.Helpers
{
    class DifficultyMatcher
    {
        public RatingRange EasyRatingRange { get; } = new RatingRange(3900, 4400);
        public RatingRange MediumRatingRange { get; } = new RatingRange(5400, 5900);
        public RatingRange HardRatingRange { get; } = new RatingRange(6900, 7400);

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
