using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SudokuGameBackend.BLL.DTO;
using SudokuGameBackend.BLL.Exceptions;
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
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IRatingService ratingService;
        private readonly IStatsService statsService;
        private readonly ILogger<UserController> logger;

        public UserController(
            IUserService userService, 
            IRatingService ratingService, 
            IStatsService statsService, 
            ILogger<UserController> logger)
        {
            this.userService = userService;
            this.ratingService = ratingService;
            this.statsService = statsService;
            this.logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser(CreateUserInput input,
            [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions)
        {
            logger.LogDebug($"CreateUser. {input}");
            UserRecord userRecord = null;
            try
            {
                if (await userService.IsUserNameAvailable(input.Name))
                {
                    userRecord = await FirebaseAuth.DefaultInstance?.CreateUserAsync(new UserRecordArgs
                    {
                        Email = input.Email,
                        Password = input.Password,
                        DisplayName = input.Email
                    });
                    await userService.AddUser(new AddUserDto
                    {
                        Id = userRecord.Uid,
                        Name = input.Name,
                        CountryCode = input.CountryCode
                    });
                    // TODO: Add catch for this.
                    await ratingService.SetInitialDuelRating(userRecord.Uid);
                    await statsService.SetInitialStats(userRecord.Uid);
                    return CreatedAtAction("GetUser", input);
                }
                else
                {
                    logger.LogDebug($"CreateUser. Name '{input.Name}' is already in use.");
                    ModelState.AddModelError("Name", "name-in-use");
                    return apiBehaviorOptions.Value.InvalidModelStateResponseFactory(ControllerContext);
                }
            }
            catch (FirebaseAuthException ex)
            {
                if (ex.AuthErrorCode == AuthErrorCode.EmailAlreadyExists)
                {
                    logger.LogDebug($"CreateUser. Email '{input.Email}' is already in use.");
                    ModelState.AddModelError("Email", "email-in-use");
                    return apiBehaviorOptions.Value.InvalidModelStateResponseFactory(ControllerContext);
                }
                else
                {
                    logger.LogWarning($"CreateUser exception. {input}, {ex}");
                }
                return BadRequest();
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

        [HttpPost("add")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddUser(AddUserInput addUserInput,
            [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions)
        {
            logger.LogDebug($"AddUser. {addUserInput}");
            try
            {
                var userRecord = await FirebaseAuth.DefaultInstance?.GetUserByEmailAsync(addUserInput.Email);
                if (!(await userService.DoesUserExist(userRecord.Uid)))
                {
                    // TODO: Improve user name generation. Avoid collisions.
                    var userName = userService.GetNameFromEmail(userRecord.Email);
                    if (await userService.IsUserNameAvailable(userName))
                    {
                        await userService.AddUser(new AddUserDto
                        {
                            Id = userRecord.Uid,
                            Name = userName
                        });
                        // TODO: Add catch for this.
                        await ratingService.SetInitialDuelRating(userRecord.Uid);
                        await statsService.SetInitialStats(userRecord.Uid);
                        return CreatedAtAction("GetUser", addUserInput);
                    }
                    else
                    {
                        logger.LogDebug($"AddUser. Name '{userName}' is already in use.");
                        ModelState.AddModelError("Name", "name-in-use");
                        return apiBehaviorOptions.Value.InvalidModelStateResponseFactory(ControllerContext);
                    }
                }
                else
                {
                    logger.LogDebug($"AddUser. User with id '{userRecord.Uid}' already exists.");
                    ModelState.AddModelError("Id", "user-already-exists");
                    return apiBehaviorOptions.Value.InvalidModelStateResponseFactory(ControllerContext);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning($"AddUser exception. {addUserInput}, {ex}");
                return BadRequest();
            }
        }

        [Authorize]
        [HttpGet(Name = "GetUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDto>> GetUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                return await userService.GetUser(userId);
            }
            catch (ItemNotFoundException ex)
            {
                logger.LogWarning($"GetUser. User not found. userId: {userId}");
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogWarning($"GetUser exception. userId: {userId}, {ex}");
                return new StatusCodeResult(500);
            }
        }

        [Authorize]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateUser(UpdateUserInput updateUserInput, 
            [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                if (!await userService.DoesUserExist(userId))
                {
                    logger.LogDebug($"UpdateUser. User does not exist. userId: {userId}");
                    return BadRequest();
                }
                else if (!await userService.IsUserNameCanBeUpdated(updateUserInput.Name, userId))
                {
                    logger.LogDebug($"UpdateUser. Name '{updateUserInput.Name}' is already in use.");
                    ModelState.AddModelError("Name", "name-in-use");
                    return apiBehaviorOptions.Value.InvalidModelStateResponseFactory(ControllerContext);
                }

                await userService.UpdateUser(userId, updateUserInput);
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogWarning($"UpdateUser exception. {updateUserInput}, userId: {userId}, {ex}");
                return BadRequest();
            }
        }

        [Authorize]
        [HttpDelete()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                if (!await userService.DoesUserExist(userId))
                {
                    logger.LogDebug($"DeleteUser. User does not exist. userId: {userId}");
                    return BadRequest();
                }

                await FirebaseAuth.DefaultInstance.DeleteUserAsync(userId);
                await userService.DeleteUser(userId);
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogWarning($"DeleteUser exception. userId: {userId}, {ex}");
                return BadRequest();
            }
        }

        [Authorize]
        [HttpGet("stats")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Dictionary<int, UserStatsItemDto>>> GetUserStats()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                return await userService.GetUserStats(userId);
            }
            catch (Exception ex)
            {
                logger.LogWarning($"GetUserStats. userId: {userId}, exception: {ex}");
                return BadRequest();
            }
        }
    }
}
