using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.BLL.DTO
{
    public class RatingDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CountryCode { get; set; }
        public int Value { get; set; }
    }
}
