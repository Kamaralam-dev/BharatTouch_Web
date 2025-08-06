using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class DeviceInfoModel
    {
        public string IPAddress { get; set; }
        public string BrowserName { get; set; }
        public string BrowserVersion { get; set; }
        public string PlateForm { get; set; }
        public string LocationLat { get; set; }
        public string LocationLon { get; set; }
        public string ModelName { get; set; }
        public string IsMobile { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string UserAgent { get; set; }
    }
}
