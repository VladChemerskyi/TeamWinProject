using SudokuGameBackend.BLL.DTO;
using SudokuGameBackend.BLL.InputModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SudokuGameBackend.BLL.Interfaces
{
    public interface IRatingService
    {
        Task<Dictionary<string, int>> CalculateDuelRatings(string winnerId, string loserId, GameMode gameMode);
        Task<bool> UpdateSolvingRating(string userId, int time, GameMode gameMode);
        Task SetInitialDuelRating(string userId);
        Task UpdateDuelRating(string userId, GameMode gameMode, int rating);
        Task<int> RemoveDuelRatingForInactivity(string userId, GameMode gameMode);
        Task<int> GetUserRating(string userId, GameMode gameMode);
        Task<int> GetUserSolvingRating(string userId, GameMode gameMode);
        Task<List<RatingDto>> GetDuelLeaderboardAsync(GetLeaderboardInput input);
        Task<List<RatingDto>> GetSolvingLeaderboardAsync(GetLeaderboardInput input);
    }
}
