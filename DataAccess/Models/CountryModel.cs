using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class CountryModel
    {
        public int CountryId { get; set; }
        public string Country { get; set; }
        public string Abbreviation { get; set; }
        public string CountryCode { get; set; }
        public string NumberCode { get; set; }
        public int MinNumberLength { get; set; }
        public int MaxNumberLength { get; set; }
        public int CreatedBy { get; set; }
    }
}
