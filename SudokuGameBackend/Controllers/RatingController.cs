using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SudokuGameBackend.BLL.DTO;
using SudokuGameBackend.BLL.InputModels;
using SudokuGameBackend.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        public async Task<ActionResult<LeaderboardDto>> GetDuelLeaderboard([FromQuery] GetLeaderboardInput input)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return await ratingService.GetDuelLeaderboardAsync(input, userId);
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
        public async Task<ActionResult<LeaderboardDto>> GetSolvingLeaderboard([FromQuery] GetLeaderboardInput input)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return await ratingService.GetSolvingLeaderboardAsync(input, userId);
            }
            catch (Exception ex)
            {
                logger.LogWarning($"GetSolvingLeaderboard exception. input: {input}, ex: {ex}");
                return BadRequest();
            }
        }
    }
}
