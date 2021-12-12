using SudokuGameBackend.BLL.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SudokuGameBackend.BLL.InputModels
{
    public class CreateUserInput
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [RegularExpression(@"^(?!.*(_)\1{1})[A-Za-z0-9_]{3,16}$")]
        public string Name { get; set; }

        [CountryCode]
        public string CountryCode { get; set; }

        public override string ToString()
        {
            return $"AddUserInput(Email: {Email}, Name: {Name}, CountryCode: {CountryCode ?? "null"})";
        }
    }
}
