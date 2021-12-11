using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SudokuGameBackend.BLL.DTO;
using SudokuGameBackend.BLL.Exceptions;
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
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IRatingService ratingService;
        private readonly ILogger<UserController> logger;

        public UserController(IUserService userService, IRatingService ratingService, ILogger<UserController> logger)
        {
            this.userService = userService;
            this.ratingService = ratingService;
            this.logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddUser(AddUserInput input)
        {
            logger.LogDebug($"AddUser. {input}");
            ActionResult result;
            if (ModelState.IsValid)
            {
                try
                {
                    await userService.AddUser(input);
                    await ratingService.SetInitialDuelRating(input.Id);
                    result = CreatedAtAction("GetUser", new { id = input.Id }, input);
                }
                catch (Exception ex)
                {
                    logger.LogWarning($"AddUser exception. {input}, {ex}");
                    result = BadRequest();
                }
            }
            else
            {
                logger.LogWarning($"AddUser. Validation failed. {input}");
                result = BadRequest(ModelState);
            }
            return result;
        }

        [HttpGet("{id}", Name = "GetUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetUser(string id)
        {
            try
            {
                return await userService.GetUser(id);
            }
            catch (ItemNotFoundException ex)
            {
                logger.LogWarning($"GetUser. User not found. userId: {id}");
                return NotFound(new { error = ex.Message });
            }
        }
    }
}
