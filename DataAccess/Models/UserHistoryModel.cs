using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class UserHistoryModel
    {
        public int ViewHistoryId { get; set; }
        public int UserId { get; set; }
        public string IPAddress { get; set; }
        public string BrowserName { get; set; }
        public string BrowserVersion { get; set; }
        public string PlateForm { get; set; }
        public string UserName { get; set; }
        public DateTime? ViewDate { get; set; }
        
    }
}
