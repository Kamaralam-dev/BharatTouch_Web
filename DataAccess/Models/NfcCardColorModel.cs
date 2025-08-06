using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class NfcCardColorModel
    {
        public int CardColorId { get; set; }
        public string Color { get; set; }
        public string ImagePath { get; set; }
        public string BackgroundColor { get; set; }
        public string TextColor { get; set; }
    }
}
