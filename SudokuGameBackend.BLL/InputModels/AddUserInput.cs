using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SudokuGameBackend.BLL.InputModels
{
    public class AddUserInput
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string CountryCode { get; set; }

        public override string ToString()
        {
            return $"AddUserInput(Id: {Id}, Name: {Name}, CountryCode: {CountryCode})";
        }
    }
}
