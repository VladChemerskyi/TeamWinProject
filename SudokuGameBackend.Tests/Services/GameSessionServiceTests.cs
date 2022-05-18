using Microsoft.Extensions.DependencyInjection;
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
    public class GameSessionServiceTests
    {
        private readonly Mock<IServiceScopeFactory> serviceScopeFactory;
        private readonly Mock<ILogger<GameSessionsService>> logger;

        public GameSessionServiceTests()
        {
            serviceScopeFactory = new Mock<IServiceScopeFactory>();
            logger = new Mock<ILogger<GameSessionsService>>();
        }

        [Fact]
        public void CreateSession_SuccessTest()
        {
            var service = new GameSessionsService(serviceScopeFactory.Object, logger.Object);
            var sessionId = service.CreateSession(GameMode.OnePuzzleEazy, "1", "2");

            Assert.True(service.TryGetSession(sessionId, out var gameSession));
            Assert.NotNull(gameSession);
            Assert.Equal(2, gameSession.UserIds.Count);
            Assert.Contains("1", gameSession.UserIds);
            Assert.Contains("2", gameSession.UserIds);
        }

        [Fact]
        public void TryGetSession_SuccessTest()
        {
            var service = new GameSessionsService(serviceScopeFactory.Object, logger.Object);
            var session1Id = service.CreateSession(GameMode.OnePuzzleEazy, "1", "2");
            var session2Id = service.CreateSession(GameMode.OnePuzzleEazy, "3");

            Assert.True(service.TryGetSession(session1Id, out var gameSession1));
            Assert.NotNull(gameSession1);
            Assert.Equal(2, gameSession1.UserIds.Count);
            Assert.Contains("1", gameSession1.UserIds);
            Assert.Contains("2", gameSession1.UserIds);

            Assert.True(service.TryGetSession(session2Id, out var gameSession2));
            Assert.NotNull(gameSession2);
            Assert.Single(gameSession2.UserIds);
            Assert.Contains("3", gameSession2.UserIds);
        }

        [Fact]
        public void DeleteSession_SuccessTest()
        {
            var service = new GameSessionsService(serviceScopeFactory.Object, logger.Object);
            var session1Id = service.CreateSession(GameMode.OnePuzzleEazy, "1", "2");
            service.DeleteSession(session1Id);
            Assert.False(service.TryGetSession(session1Id, out _));
        }
    }
}
