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
        [RegularExpression(@"^(?!.*(_)\1{1})[A-Za-z0-9_]{3,16}$")]
        public string Name { get; set; }

        [CountryCode]
        public string CountryCode { get; set; }
    }
}
