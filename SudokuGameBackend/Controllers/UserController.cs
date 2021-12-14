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
                        Password = input.Password
                    });
                    await userService.AddUser(new AddUserDto
                    {
                        Id = userRecord.Uid,
                        Name = input.Name,
                        CountryCode = input.CountryCode
                    });
                    // TODO: Add catch for this.
                    await ratingService.SetInitialDuelRating(userRecord.Uid);
                    return CreatedAtAction("GetUser", new { id = userRecord.Uid }, input);
                }
                else
                {
                    logger.LogDebug($"CreateUser. Name '{input.Name}' is already in use.");
                    ModelState.AddModelError("Name", $"Name '{input.Name}' is already in use.");
                    return apiBehaviorOptions.Value.InvalidModelStateResponseFactory(ControllerContext);
                }
            }
            catch (FirebaseAuthException ex)
            {
                if (ex.AuthErrorCode == AuthErrorCode.EmailAlreadyExists)
                {
                    logger.LogDebug($"CreateUser. Email '{input.Email}' is already in use.");
                    ModelState.AddModelError("Email", ex.Message);
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
                        return CreatedAtAction("GetUser", new { id = userRecord.Uid }, addUserInput);
                    }
                    else
                    {
                        logger.LogDebug($"AddUser. Name '{userName}' is already in use.");
                        ModelState.AddModelError("Name", $"Name '{userName}' is already in use.");
                        return apiBehaviorOptions.Value.InvalidModelStateResponseFactory(ControllerContext);
                    }
                }
                else
                {
                    logger.LogDebug($"AddUser. User with id '{userRecord.Uid}' already exists.");
                    ModelState.AddModelError("Id", $"User with id '{userRecord.Uid}' already exists.");
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
