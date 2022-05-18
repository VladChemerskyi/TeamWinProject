using Microsoft.Extensions.Logging;
using Moq;
using SudokuGameBackend.BLL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SudokuGameBackend.Tests.Services
{
    public class MatchmakingServiceTests
    {
        private readonly Mock<ILogger<MatchmakingService>> logger;

        public MatchmakingServiceTests()
        {
            logger = new Mock<ILogger<MatchmakingService>>();
        }

        [Fact]
        public void TryFindOpponent_SuccessTest()
        {
            var service = new MatchmakingService(logger.Object);
            service.AddToQueue("1", 1000, GameMode.OnePuzzleEazy);
            service.AddToQueue("2", 3000, GameMode.OnePuzzleEazy);
            service.AddToQueue("3", 1000, GameMode.OnePuzzleEazy);

            var result = service.TryFindOpponent(GameMode.OnePuzzleEazy, 1000, "3", out var opponentId);
            Assert.True(result);
            Assert.Equal("1", opponentId);
        }

        [Fact]
        public void RemoveFromQueue_SuccessTest()
        {
            var service = new MatchmakingService(logger.Object);
            service.AddToQueue("1", 1000, GameMode.OnePuzzleEazy);

            service.RemoveFromQueue("1");

            var result = service.TryFindOpponent(GameMode.OnePuzzleEazy, 1000, "2", out _);
            Assert.False(result);
        }

        [Fact]
        public void AddToQueue_SuccessTest()
        {
            var service = new MatchmakingService(logger.Object);

            var result = service.TryFindOpponent(GameMode.OnePuzzleEazy, 1000, "2", out _);
            Assert.False(result);

            service.AddToQueue("1", 1000, GameMode.OnePuzzleEazy);

            result = service.TryFindOpponent(GameMode.OnePuzzleEazy, 1000, "2", out var opponentId);
            Assert.True(result);
            Assert.Equal("1", opponentId);
        }
    }
}
