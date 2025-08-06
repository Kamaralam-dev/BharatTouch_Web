using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class AviPdfUserModel
    {
        public int id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }   
        public string Password { get; set; }
        public string DeviceID { get; set; }
        public string DeviceType { get; set; }
        public string ProfilePicture { get; set; }  
        public DateTime? CreatedOn { get; set; }
        public bool Status { get; set; }
    }
}
