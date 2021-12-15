using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SudokuGameBackend.BLL.DTO;
using SudokuGameBackend.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SudokuGameBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly ILogger<CountriesController> logger;
        private readonly ICountriesService countriesService;

        public CountriesController(ILogger<CountriesController> logger, ICountriesService countriesService)
        {
            this.logger = logger;
            this.countriesService = countriesService;
        }

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<CountryDto>>> GetAllCountries()
        {
            try
            {
                return await countriesService.GetAllCountries();
            }
            catch (Exception ex)
            {
                logger.LogWarning($"GetAllCountries exception. ex: {ex}");
                return BadRequest();
            }
        }
    }
}
