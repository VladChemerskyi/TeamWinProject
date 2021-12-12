using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SudokuGameBackend.BLL.DTO;
using SudokuGameBackend.BLL.InputModels;
using SudokuGameBackend.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SudokuGameBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService ratingService;
        private readonly ILogger<RatingController> logger;

        public RatingController(IRatingService ratingService, ILogger<RatingController> logger)
        {
            this.ratingService = ratingService;
            this.logger = logger;
        }

        [HttpGet("duel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<RatingDto>>> GetDuelLeaderboard([FromQuery] GetLeaderboardInput input)
        {
            try
            {
                return await ratingService.GetDuelLeaderboardAsync(input);
            }
            catch (Exception ex)
            {
                logger.LogWarning($"GetDuelLeaderboard exception. input: {input}, ex: {ex}");
                return BadRequest();
            }
        }

        [HttpGet("solving")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<RatingDto>>> GetSolvingLeaderboard([FromQuery] GetLeaderboardInput input)
        {
            try
            {
                return await ratingService.GetSolvingLeaderboardAsync(input);
            }
            catch (Exception ex)
            {
                logger.LogWarning($"GetSolvingLeaderboard exception. input: {input}, ex: {ex}");
                return BadRequest();
            }
        }
    }
}
