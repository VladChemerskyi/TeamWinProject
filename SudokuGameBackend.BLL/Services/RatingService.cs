using SudokuGameBackend.BLL.Interfaces;
using SudokuGameBackend.DAL.Entities;
using SudokuGameBackend.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.BLL.Services
{
    public class RatingService : IRatingService
    {
        private static readonly int coefficient = 16;
        private readonly IUnitOfWork unitOfWork;

        public RatingService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Dictionary<string, int> CalculateAndSaveDuelRatings(string winnerId, string loserId, GameMode gameMode)
        {
            var winnerRating = unitOfWork.DuelRatingRepository.Get(winnerId, gameMode);
            var loserRating = unitOfWork.DuelRatingRepository.Get(loserId, gameMode);

            int newWinnerRating = CalculateNewWinnerRating(winnerRating.Rating, loserRating.Rating);
            int newLoserRating = CalculateNewLoserRating(loserRating.Rating, winnerRating.Rating);

            winnerRating.Rating = newWinnerRating;
            loserRating.Rating = newLoserRating;
            unitOfWork.DuelRatingRepository.Update(winnerRating);
            unitOfWork.DuelRatingRepository.Update(loserRating);
            unitOfWork.Save();

            return new Dictionary<string, int>
            {
                { winnerId, winnerRating.Rating },
                { loserId,  loserRating.Rating  }
            };
        }

        private int CalculateUserRating(int currentUserRating, int currentOpponentRating, bool isWinner)
        {
            double expected = 1 / (1 + Math.Pow(10, (currentOpponentRating - currentUserRating) / 400));
            int newUserRating = currentUserRating + (int)Math.Round(coefficient * (isWinner ? 1 : 0 - expected));
            return newUserRating;
        }

        private int CalculateNewWinnerRating(int winnerRating, int loserRating)
        {
            return CalculateUserRating(winnerRating, loserRating, true);
        }

        private int CalculateNewLoserRating(int loserRating, int winnerRating)
        {
            return CalculateUserRating(loserRating, winnerRating, false);
        }

        public void UpdateSolvingRating(string userId, int time, GameMode gameMode)
        {
            var rating = unitOfWork.SolvingRatingRepository.Get(userId, gameMode);
            if (rating != null)
            {
                if (time < rating.Time)
                {
                    rating.Time = time;
                }
                unitOfWork.SolvingRatingRepository.Update(rating);
            }
            else
            {
                unitOfWork.SolvingRatingRepository.Create(new SolvingRating
                {
                    UserId = userId,
                    GameMode = gameMode,
                    Time = time
                });
            }
            unitOfWork.Save();
        }

        public void SetInitialDuelRating(string userId)
        {
            foreach (GameMode gameMode in Enum.GetValues(typeof(GameMode)))
            {
                var rating = unitOfWork.DuelRatingRepository.Get(userId, gameMode);
                if (rating == null)
                {
                    unitOfWork.DuelRatingRepository.Create(new DuelRating
                    {
                        UserId = userId,
                        GameMode = gameMode,
                        Rating = 1400
                    });
                }
            }
            unitOfWork.Save();
        }
    }
}
