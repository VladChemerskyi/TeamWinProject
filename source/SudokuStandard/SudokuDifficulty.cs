using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SudokuStandard
{
    public enum DifficultyLevel
    {
        Beginner = 1,
        Easy,
        Medium,
        Tricky,
        Fiendish,
        Diabolical
    }

    public struct RatingRange
    {
        [JsonInclude]
        public int MinRating { get; private set; }
        [JsonInclude]
        public int MaxRating { get; private set; }
        public double Average { get; private set; }

        public RatingRange(int minRating, int maxRating)
        {
            MinRating = minRating;
            MaxRating = maxRating;
            Average = (minRating + maxRating) / 2d;
        }

        public bool Matches(int rating)
        {
            return rating >= MinRating && rating <= MaxRating;
        }

        public static RatingRange FromDifficultyLevel(DifficultyLevel difficultyLevel)
        {
            RatingRange ratingRange;
            switch (difficultyLevel)
            {
                case DifficultyLevel.Beginner:
                    ratingRange = new RatingRange(3600, 4500);
                    break;
                case DifficultyLevel.Easy:
                    ratingRange = new RatingRange(4300, 5500);
                    break;
                case DifficultyLevel.Medium:
                    ratingRange = new RatingRange(5300, 6900);
                    break;
                case DifficultyLevel.Tricky:
                    ratingRange = new RatingRange(6500, 9300);
                    break;
                case DifficultyLevel.Fiendish:
                    ratingRange = new RatingRange(8300, 14000);
                    break;
                case DifficultyLevel.Diabolical:
                    ratingRange = new RatingRange(11000, 25000);
                    break;
                default:
                    ratingRange = RatingRange.Default();
                    break;
            }
            return ratingRange;
        }

        public static RatingRange Default()
        {
            return new RatingRange(0, int.MaxValue);
        }
    }
}
