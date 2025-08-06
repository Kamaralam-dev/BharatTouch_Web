using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class DeviceTokenModel
    {
        public int DeviceToken_Id { get; set; }
        public string Device_Id { get; set; }
        public string Device_Token { get; set; }
        public string DeviceDescription { get; set; }
        public int UserId { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
