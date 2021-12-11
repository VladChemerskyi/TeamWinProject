﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public UserController(IUserService userService, IRatingService ratingService)
        {
            this.userService = userService;
            this.ratingService = ratingService;
        }

        [Authorize]
        [HttpPost]
        [Route("add")]
        public void AddUser(AddUserInput input)
        {
            userService.AddUser(input);
            ratingService.SetInitialDuelRating(input.Id);
        }
    }
}