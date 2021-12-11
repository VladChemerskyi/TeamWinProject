using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuGameBackend.DAL.Entities
{
    public class User
    {
        public string Id { get; set; }
        public string CountryCode { get; set; }
        public string Name { get; set; }

        public List<DuelRating> DuelRatings { get; set; }
        public List<SolvingRating> SolvingRatings { get; set; }
    }
}
