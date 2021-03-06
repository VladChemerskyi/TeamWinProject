using SudokuGameBackend.BLL.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SudokuGameBackend.BLL.InputModels
{
    public class CreateUserInput : UserInputBase
    {
        [Required]
        [EmailAddress(ErrorMessage = "invalid-email")]
        public string Email { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 6, ErrorMessage = "password-incorrect-format")]
        public string Password { get; set; }

        public override string ToString()
        {
            return $"AddUserInput(Email: {Email}, Name: {Name}, CountryCode: {CountryCode ?? "null"})";
        }
    }
}
