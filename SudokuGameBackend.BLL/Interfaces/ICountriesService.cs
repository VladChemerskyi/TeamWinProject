using SudokuGameBackend.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SudokuGameBackend.BLL.Interfaces
{
    public interface ICountriesService
    {
        Task<CountryDto> GetCountryByCode(string code);
        Task<List<CountryDto>> GetAllCountries();
    }
}
