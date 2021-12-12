using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SudokuGameBackend.BLL.InputModels
{
    public class CreateUserInput
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Name { get; set; }
        public string CountryCode { get; set; }

        public override string ToString()
        {
            return $"AddUserInput(Email: {Email}, Name: {Name}, CountryCode: {CountryCode})";
        }
    }
}
