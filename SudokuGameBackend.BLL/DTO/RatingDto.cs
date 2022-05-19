using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SudokuGameBackend.BLL.DTO
{
    public class RatingDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("countryCode")]
        public string CountryCode { get; set; }

        [JsonPropertyName("value")]
        public int Value { get; set; }

        [JsonPropertyName("place")]
        public int Place { get; set; }
    }
}
