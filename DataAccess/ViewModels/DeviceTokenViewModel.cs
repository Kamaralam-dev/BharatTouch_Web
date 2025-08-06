using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModels
{
    public class DeviceTokenViewModel
    {
        public int UserId { get; set; }
        public string DeviceId { get; set; }
        public string FirebaseToken { get; set; }
        public string DeviceType { get; set; }
        public string PhoneMake { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    }
}
