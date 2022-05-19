using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using SudokuGameBackend.BLL.DTO;
using SudokuGameBackend.BLL.Interfaces;
using SudokuGameBackend.BLL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SudokuGameBackend.Tests.Services
{
    public class CountriesServiceTests
    {
        private readonly Mock<ICacheService> cacheService;
        private readonly Mock<ILogger<CountriesService>> logger;
        private readonly Mock<IHttpClientFactory> httpClientFactory;
        private readonly Mock<HttpMessageHandler> httpMessageHandler;

        public CountriesServiceTests()
        {
            cacheService = new Mock<ICacheService>();
            logger = new Mock<ILogger<CountriesService>>();
            httpClientFactory = new Mock<IHttpClientFactory>();
            httpMessageHandler = new Mock<HttpMessageHandler>();
        }

        [Fact]
        public async Task GetCountryByCode_SuccessTest()
        {
            httpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"alpha2Code\": \"UA\", \"nativeName\": \"Ukraine\"}")
                });

            httpClientFactory
                .Setup(x => x.CreateClient(
                    It.IsAny<string>()))
                .Returns(new HttpClient(httpMessageHandler.Object));

            var service = new CountriesService(cacheService.Object, logger.Object, httpClientFactory.Object);
            var result = await service.GetCountryByCode("UA");

            Assert.NotNull(result);
            Assert.Equal("UA", result.Alpha2Code);
            Assert.Equal("Ukraine", result.NativeName);
        }

        [Fact]
        public async Task GetCountryByCode_FailedTest()
        {
            httpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException());

            httpClientFactory
                .Setup(x => x.CreateClient(
                    It.IsAny<string>()))
                .Returns(new HttpClient(httpMessageHandler.Object));

            var service = new CountriesService(cacheService.Object, logger.Object, httpClientFactory.Object);
            var result = await service.GetCountryByCode("UA");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllCountries_SuccessTest()
        {
            cacheService
                .Setup(x => x.GetOrCreateAsync(
                    It.IsAny<string>(),
                    It.IsAny<TimeSpan>(),
                    It.IsAny<Func<Task<List<CountryDto>>>>()))
                .ReturnsAsync(new List<CountryDto> { new CountryDto { Alpha2Code = "UA" } });

            var service = new CountriesService(cacheService.Object, logger.Object, httpClientFactory.Object);

            var result = await service.GetAllCountries();

            Assert.NotNull(result);
            var country = Assert.Single(result);
            Assert.Equal("UA", country.Alpha2Code);
        }

        [Fact]
        public async Task GetAllCountries_FailedTest()
        {
            httpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException());

            httpClientFactory
                .Setup(x => x.CreateClient(
                    It.IsAny<string>()))
                .Returns(new HttpClient(httpMessageHandler.Object));

            var realCahceService = new CacheService(
                new MemoryCache(new MemoryCacheOptions()), 
                new Mock<ILogger<CacheService>>().Object);

            var service = new CountriesService(realCahceService, logger.Object, httpClientFactory.Object);

            var result = await service.GetAllCountries();

            Assert.Null(result);
        }
    }
}
