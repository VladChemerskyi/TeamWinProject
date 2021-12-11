﻿using SudokuGameBackend.BLL.Interfaces;
using SudokuGameBackend.DAL.Entities;
using SudokuGameBackend.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SudokuGameBackend.BLL.Services
{
    public class RatingService : IRatingService
    {
        private readonly int maxRatingDeltaForGame = 16;
        private readonly IUnitOfWork unitOfWork;

        public RatingService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<Dictionary<string, int>> CalculateDuelRatings(string winnerId, string loserId, GameMode gameMode)
        {
            var winnerRating = await unitOfWork.DuelRatingRepository.GetAsync(winnerId, gameMode);
            var loserRating = await unitOfWork.DuelRatingRepository.GetAsync(loserId, gameMode);

            int newWinnerRating = CalculateNewWinnerRating(winnerRating.Rating, loserRating.Rating);
            int newLoserRating = CalculateNewLoserRating(loserRating.Rating, winnerRating.Rating);

            return new Dictionary<string, int>
            {
                { winnerId, newWinnerRating },
                { loserId,  newLoserRating  }
            };
        }

        public async Task UpdateDuelRating(string userId, GameMode gameMode, int rating)
        {
            var duelRating = await unitOfWork.DuelRatingRepository.GetAsync(userId, gameMode);
            duelRating.Rating = rating;
            unitOfWork.DuelRatingRepository.Update(duelRating);
            await unitOfWork.SaveAsync();
        }

        private int CalculateUserRating(int currentUserRating, int currentOpponentRating, bool isWinner)
        {
            double expected = 1 / (1 + Math.Pow(10, (currentOpponentRating - currentUserRating) / 400));
            int newUserRating = currentUserRating + (int)Math.Round(maxRatingDeltaForGame * (isWinner ? 1 : 0 - expected));
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

        public async Task UpdateSolvingRating(string userId, int time, GameMode gameMode)
        {
            var rating = await unitOfWork.SolvingRatingRepository.GetAsync(userId, gameMode);
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
                await unitOfWork.SolvingRatingRepository.CreateAsync(new SolvingRating
                {
                    UserId = userId,
                    GameMode = gameMode,
                    Time = time
                });
            }
            await unitOfWork.SaveAsync();
        }

        public async Task SetInitialDuelRating(string userId)
        {
            foreach (GameMode gameMode in Enum.GetValues(typeof(GameMode)))
            {
                var rating = await unitOfWork.DuelRatingRepository.GetAsync(userId, gameMode);
                if (rating == null)
                {
                    await unitOfWork.DuelRatingRepository.CreateAsync(new DuelRating
                    {
                        UserId = userId,
                        GameMode = gameMode,
                        Rating = 1400
                    });
                }
            }
            await unitOfWork.SaveAsync();
        }

        public async Task RemoveDuelRatingForInactivity(string userId, GameMode gameMode)
        {
            var currentRating = await unitOfWork.DuelRatingRepository.GetAsync(userId, gameMode);
            currentRating.Rating -= maxRatingDeltaForGame;
            unitOfWork.DuelRatingRepository.Update(currentRating);
            await unitOfWork.SaveAsync();
        }
    }
}
