using SudokuGameBackend.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace SudokuGameBackend.IntegrationTests
{
    public class PuzzleTests : BaseTests
    {
        [Theory]
        [InlineData("1", 1)]
        [InlineData("2", 1)]
        [InlineData("3", 1)]
        [InlineData("4", 3)]
        public async Task GetPuzzleTest(string gameMode, int puzzleCount)
        {
            using var client = _server.CreateClient();
            var response = await client.GetAsync($"api/puzzle?gameMode={gameMode}");
            var stringData = await response.Content.ReadAsStringAsync();
            var puzzles = JsonSerializer.Deserialize<List<RegularSudokuDto>>(stringData);

            Assert.NotNull(puzzles);
            Assert.Equal(puzzleCount, puzzles.Count);
        }
    }
}
