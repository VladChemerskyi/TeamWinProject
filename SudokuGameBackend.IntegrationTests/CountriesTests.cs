using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using SudokuGameBackend.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace SudokuGameBackend.IntegrationTests
{

    public class CountriesTests : BaseTests
    {
        [Fact]
        public async Task GetAllCountriesTest()
        {
            using var client = _server.CreateClient();
            var response = await client.GetAsync("api/countries/all");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var stringResult = await response.Content.ReadAsStringAsync();
            var countries = JsonSerializer.Deserialize<List<CountryDto>>(stringResult);
            Assert.NotEmpty(countries);
            Assert.NotNull(countries[0].Alpha2Code);
            Assert.NotNull(countries[0].NativeName);
        }
    }
}
