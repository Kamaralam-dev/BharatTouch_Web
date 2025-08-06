using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class ProfileCompletionEmailModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public int ProfileCompletePercent { get; set; }
        public string EmailId { get; set; }
        public string Displayname { get; set; }
        public string EmailSubject { get; set; }
    }
}
