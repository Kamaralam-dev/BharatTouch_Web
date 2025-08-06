using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.AdminApiDto
{
    public class UpsertDeviceTokenDto
    {
        public string Device_Id { get; set; }
        public string Device_Token { get; set; }
        public string DeviceDescription { get; set; }
        public int UserId { get; set; }
    }

    public class RemoveDeviceTokenDto
    {
        public int UserId { get; set; }
        public string Device_Id { get; set; }
    }
}
