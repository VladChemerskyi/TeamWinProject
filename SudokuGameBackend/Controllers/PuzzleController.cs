using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SudokuGameBackend.BLL.Interfaces;
using SudokuGameBackend.BLL.DTO;

namespace SudokuGameBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PuzzleController : ControllerBase
    {
        private readonly ILogger<PuzzleController> logger;
        private readonly IPuzzleService puzzleService;

        public PuzzleController(ILogger<PuzzleController> logger, IPuzzleService puzzleService)
        {
            this.logger = logger;
            this.puzzleService = puzzleService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<RegularSolvedSudokuDto>>> GetPuzzle(GameMode gameMode)
        {
            try
            {
                return await puzzleService.GetRegularPuzzle(gameMode);
            }
            catch (Exception ex)
            {
                logger.LogWarning($"GetPuzzle exception. gameMode: {gameMode}, exception: {ex}");
                return StatusCode(500);
            }
        }
    }
}
