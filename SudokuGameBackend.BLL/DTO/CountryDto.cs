using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SudokuGameBackend.BLL.DTO
{
    public class CountryDto
    {
        [JsonPropertyName("alpha2Code")]
        public string Alpha2Code { get; set; }
        [JsonPropertyName("nativeName")]
        public string NativeName { get; set; }
    }
}
