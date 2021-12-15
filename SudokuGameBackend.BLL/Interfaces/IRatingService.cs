using SudokuGameBackend.BLL.DTO;
using SudokuGameBackend.BLL.Helpers;
using SudokuGameBackend.BLL.InputModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SudokuGameBackend.BLL.Interfaces
{
    public interface IRatingService
    {
        Task<bool> UpdateSolvingRating(string userId, int time, GameMode gameMode);
        Task SetInitialDuelRating(string userId);
        Task<int> RemoveDuelRatingForInactivity(string userId, GameMode gameMode);
        Task<int> GetUserRating(string userId, GameMode gameMode);
        Task<int> GetUserSolvingRating(string userId, GameMode gameMode);
        Task<List<RatingDto>> GetDuelLeaderboardAsync(GetLeaderboardInput input);
        Task<List<RatingDto>> GetSolvingLeaderboardAsync(GetLeaderboardInput input);
        Task<IRatings> UpdateUsersRatings(
            string user1Id, GameResultType user1GameResult, string user2Id, GameResultType user2GameResult, GameMode gameMode);
    }

    public interface IRatings
    {
        Dictionary<string, int> OldRatings { get; set; }
        Dictionary<string, int> NewRatings { get; set; }
    }
}
