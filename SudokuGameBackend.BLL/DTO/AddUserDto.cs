using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.BLL.DTO
{
    public class AddUserDto
    {
        public string Id { get; set; }
        public string CountryCode { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"AddUserDto(Id: {Id}, CountryCode: {CountryCode}, Name: {Name})";
        }
    }
}
