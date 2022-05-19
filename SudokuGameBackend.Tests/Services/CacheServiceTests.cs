using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using SudokuGameBackend.BLL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SudokuGameBackend.Tests.Services
{
    public class CacheServiceTests
    {
        private readonly IMemoryCache memoryCache;
        private readonly Mock<ILogger<CacheService>> logger;

        public CacheServiceTests()
        {
            memoryCache = new MemoryCache(new MemoryCacheOptions());
            logger = new Mock<ILogger<CacheService>>();
        }

        [Fact]
        public async Task GetOrCreateAsync_SuccessTest()
        {
            var service = new CacheService(memoryCache, logger.Object);

            var result = await service.GetOrCreateAsync("key", TimeSpan.FromSeconds(1), async () => await Task.FromResult(42));
            Assert.Equal(42, result);

            Thread.Sleep(1000);

            result = await service.GetOrCreateAsync("key", TimeSpan.FromSeconds(5), async () => await Task.FromResult(43));
            Assert.Equal(43, result);

            result = await service.GetOrCreateAsync("key", TimeSpan.FromSeconds(5), async () => await Task.FromResult(44));
            Assert.Equal(43, result);
        }
    }
}
