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
    public class PuzzleServiceTests
    {
        private readonly Mock<ILogger<PuzzleService>> logger;

        public PuzzleServiceTests()
        {
            logger = new Mock<ILogger<PuzzleService>>();
        }

        [Fact]
        public async Task GetRegularPuzzle_SuccessTest()
        {
            var service = new PuzzleService(logger.Object);

            var puzzles = await service.GetRegularPuzzle(GameMode.OnePuzzleEazy);
            Assert.NotNull(puzzles);
            var puzzle = Assert.Single(puzzles);
            Assert.NotNull(puzzle);
            Assert.NotNull(puzzle.BoardArray);
            Assert.Contains(puzzle.BoardArray, number => number != 0);
            Assert.DoesNotContain(puzzle.SolutionArray, number => number == 0);

            puzzles = await service.GetRegularPuzzle(GameMode.ThreePuzzles);
            Assert.NotNull(puzzles);
            Assert.Equal(3, puzzles.Count);
        }
    }
}
