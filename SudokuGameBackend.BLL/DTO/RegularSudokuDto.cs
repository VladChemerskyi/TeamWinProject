using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SudokuGameBackend.BLL.DTO
{
    public class RegularSudokuDto
    {
        public int Id { get; set; }
        public int[] BoardArray { get; set; }

        [JsonIgnore]
        public bool IsValid
        {
            get => BoardArray != null;
        }
    }
}
