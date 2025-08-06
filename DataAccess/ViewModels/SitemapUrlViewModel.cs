using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModels
{
    public class SitemapUrlViewModel
    {
        public string Loc { get; set; }
        public DateTime? LastMod { get; set; }
        public string Priority { get; set; }
    }
}
