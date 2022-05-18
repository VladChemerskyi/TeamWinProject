using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SudokuGameBackend.BLL.DTO;
using SudokuGameBackend.BLL.Interfaces;
using SudokuGameBackend.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SudokuGameBackend.Tests.Controllers
{
    public class CountriesControllerTests
    {
        private readonly Mock<ILogger<CountriesController>> logger;
        private readonly Mock<ICountriesService> countriesService;

        public CountriesControllerTests()
        {
            logger = new Mock<ILogger<CountriesController>>();
            countriesService = new Mock<ICountriesService>();
        }

        [Fact]
        public async Task GetAllCountries_SuccessTest()
        {
            countriesService
                .Setup(x => x.GetAllCountries())
                .ReturnsAsync(new List<CountryDto> { new CountryDto() });

            var controller = new CountriesController(logger.Object, countriesService.Object);

            var result = await controller.GetAllCountries();
            Assert.NotNull(result);
            var countries = Assert.IsType<List<CountryDto>>(result.Value);
            var country = Assert.Single(countries);
            Assert.NotNull(country);
        }

        [Fact]
        public async Task GetAllCountries_FailedTest()
        {
            countriesService
                .Setup(x => x.GetAllCountries())
                .ThrowsAsync(new Exception());

            var controller = new CountriesController(logger.Object, countriesService.Object);

            var result = await controller.GetAllCountries();
            Assert.NotNull(result);
            Assert.Null(result.Value);
        }
    }
}
