using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace SudokuGameBackend.BLL.ValidationAttributes
{
    class CountryCodeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value != null)
            {
                return value is string stringValue && ISO3166.Country.List.Any(country => 
                {
                    return country.TwoLetterCode == stringValue;
                });
            }
            return true;
        }
    }
}
