using SudokuGameBackend.BLL.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SudokuGameBackend.BLL.InputModels
{
    public class AddUserInput
    {
        [Required]
        [EmailAddress(ErrorMessage = "invalid-email")]
        public string Email { get; set; }
    }
}
