using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class RnauraTeamModel
    {
        public int TeamId { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string About { get; set; }
        public string Image { get; set; }
        public int DisplayPosition { get; set; }
    }
}
