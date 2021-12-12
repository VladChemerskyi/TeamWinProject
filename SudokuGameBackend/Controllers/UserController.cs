using FirebaseAdmin.Auth;
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
        public async Task<ActionResult> CreateUser(CreateUserInput input)
        {
            logger.LogDebug($"CreateUser. {input}");
            UserRecord userRecord = null;
            try
            {
                userRecord = await FirebaseAuth.DefaultInstance?.CreateUserAsync(new UserRecordArgs 
                {
                    Email = input.Email,
                    Password = input.Password
                });
                await userService.AddUser(new AddUserDto
                {
                    Id = userRecord.Uid,
                    Name = input.Name,
                    CountryCode = input.CountryCode
                });
                await ratingService.SetInitialDuelRating(userRecord.Uid);
                return CreatedAtAction("GetUser", new { id = userRecord.Uid }, input);
            }
            catch (UserAddingException ex)
            {
                logger.LogWarning($"CreateUser exception. {input}, {ex}");
                if (userRecord != null)
                {
                    await FirebaseAuth.DefaultInstance?.DeleteUserAsync(userRecord.Uid);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                logger.LogWarning($"CreateUser exception. {input}, {ex}");
                return BadRequest();
            }
        }

        [Authorize]
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
