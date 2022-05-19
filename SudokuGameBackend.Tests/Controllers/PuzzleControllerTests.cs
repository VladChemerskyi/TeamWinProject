using Microsoft.Extensions.Logging;
using Moq;
using SudokuGameBackend.BLL.DTO;
using SudokuGameBackend.BLL.Interfaces;
using SudokuGameBackend.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SudokuGameBackend.Tests.Controllers
{
    public class PuzzleControllerTests
    {
        private readonly Mock<ILogger<PuzzleController>> logger;
        private readonly Mock<IPuzzleService> puzzleService;

        public PuzzleControllerTests()
        {
            logger = new Mock<ILogger<PuzzleController>>();
            puzzleService = new Mock<IPuzzleService>();
        }

        [Fact]
        public async Task GetPuzzle_SuccessTest()
        {
            puzzleService
                .Setup(x => x.GetRegularPuzzle(
                    It.IsAny<GameMode>()))
                .ReturnsAsync(new List<RegularSolvedSudokuDto> { new RegularSolvedSudokuDto() });

            var controller = new PuzzleController(logger.Object, puzzleService.Object);
            var result = await controller.GetPuzzle(GameMode.OnePuzzleEazy);

            Assert.NotNull(result);
            var puzzles = Assert.IsType<List<RegularSolvedSudokuDto>>(result.Value);
            var puzzle = Assert.Single(puzzles);
            Assert.NotNull(puzzle);
        }

        [Fact]
        public async Task GetPuzzle_FailedTest()
        {
            puzzleService
                .Setup(x => x.GetRegularPuzzle(
                    It.IsAny<GameMode>()))
                .ThrowsAsync(new Exception());

            var controller = new PuzzleController(logger.Object, puzzleService.Object);
            var result = await controller.GetPuzzle(GameMode.OnePuzzleEazy);

            Assert.NotNull(result);
            Assert.Null(result.Value);
        }
    }
}
