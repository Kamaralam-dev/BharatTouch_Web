using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.AdminApiDto
{
    public class AuthenticateAppUserDto
    {
        public string EmailId { get; set; }
        public string Password { get; set; }
        public string DeviceId { get; set; }
        public string FirebaseToken { get; set; }
        public string DeviceType { get; set; }
        public string PhoneMake { get; set; }
    }
}
