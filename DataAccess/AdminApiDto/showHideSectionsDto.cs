using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.AdminApiDto
{
    public class showHideSectionsDto
    {
        public int UserId { get; set; }
        public string Type { get; set; }
    }

    public class PersonalInfoFileDeleteDTO : showHideSectionsDto
    {
        public string filePath { get; set; }
    }
}
