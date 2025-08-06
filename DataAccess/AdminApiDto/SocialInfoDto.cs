using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.AdminApiDto
{
    public class SocialInfoDto
    {
        public int UserId { get; set; }
        public string LinkedIn { get; set; }
        public string Twitter { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public string Skype { get; set; }   // Google Review
        public string Youtube { get; set; }
    }
}
