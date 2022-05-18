using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SudokuGameBackend.BLL.DTO;
using SudokuGameBackend.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SudokuGameBackend.BLL.Services
{
    public class CountriesService : ICountriesService
    {
        private const string baseUrl = "https://restcountries.com/v2";
        private readonly ICacheService cacheService;
        private readonly ILogger<CountriesService> logger;
        private readonly IHttpClientFactory httpClientFactory;

        public CountriesService(ICacheService cacheService, ILogger<CountriesService> logger, IHttpClientFactory httpClientFactory)
        {
            this.cacheService = cacheService;
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<List<CountryDto>> GetAllCountries()
        {
            try
            {
                return await cacheService.GetOrCreateAsync(CacheKeys.AllCountries, TimeSpan.FromDays(1), async () => 
                {
                    using var client = httpClientFactory.CreateClient();
                    var reponse = await client.GetStringAsync($"{baseUrl}/all?fields=nativeName,alpha2Code");
                    return JsonConvert.DeserializeObject<List<CountryDto>>(reponse);
                });
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning($"GetAllCountries request error. {ex}");
                return null;
            }
            catch (Exception ex)
            {
                logger.LogWarning($"GetAllCountries exception. {ex}");
                return null;
            }
        }

        public async Task<CountryDto> GetCountryByCode(string code)
        {
            try
            {
                using var client = httpClientFactory.CreateClient();
                var reponse = await client.GetStringAsync($"{baseUrl}/alpha/{code}?fields=nativeName,alpha2Code");
                return JsonConvert.DeserializeObject<CountryDto>(reponse);
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning($"GetCountryByCode request error. {ex}");
                return null;
            }
            catch (Exception ex)
            {
                logger.LogWarning($"GetCountryByCode exception. {ex}");
                return null;
            }
        }
    }
}
