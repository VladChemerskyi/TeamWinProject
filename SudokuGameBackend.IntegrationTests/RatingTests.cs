using SudokuGameBackend.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace SudokuGameBackend.IntegrationTests
{
    public class RatingTests : BaseTests
    {
        [Theory]
        [InlineData("duel")]
        [InlineData("solving")]
        public async Task GetDuelLeaderboardTest(string ratingType)
        {
            using var clien = _server.CreateClient();
            var response = await clien.GetAsync($"api/rating/{ratingType}?gameMode=1");
            var stringData = await response.Content.ReadAsStringAsync();
            var leadedrboard = JsonSerializer.Deserialize<LeaderboardDto>(stringData);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(leadedrboard);
            Assert.Null(leadedrboard.CalibrationGamesLeft);
            Assert.Null(leadedrboard.CurrentPlace);
            Assert.NotNull(leadedrboard.Ratings);
        }
    }
}
