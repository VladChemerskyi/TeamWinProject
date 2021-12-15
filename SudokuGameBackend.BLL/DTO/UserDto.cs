using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.BLL.DTO
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public CountryDto Country { get; set; }
    }
}
