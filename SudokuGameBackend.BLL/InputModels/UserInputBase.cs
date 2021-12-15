using SudokuGameBackend.BLL.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SudokuGameBackend.BLL.InputModels
{
    public abstract class UserInputBase
    {
        [Required]
        [RegularExpression(@"^[A-Za-z0-9]{3,16}$", ErrorMessage = "name-incorrect-format")]
        public string Name { get; set; }

        [CountryCode(ErrorMessage = "invalid-country-code")]
        public string CountryCode { get; set; }
    }
}
