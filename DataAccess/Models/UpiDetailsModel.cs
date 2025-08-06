using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class UpiDetailsModel
    {
        public int Id { get; set; }
        public string UpiId { get; set; }
        public string PayeeName { get; set; }
        public int UserId { get; set; }
        public string QrImage { get; set; }
        public string QrImageThumbnailPath { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
