using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModels
{
    public class RequestItemViewModel
    {
        public int RequestId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public DateTime? RequestDate { get; set; }
        public string RequestNote { get; set; }
        public string ItemNumber { get; set; }
        public int ReceiverId { get; set; }
        public string RequestBy { get; set; }
		public string DonatedBy { get; set; }
        public int DonorId { get; set; }
        public int TotalRows { get; set; }
        public string RequestType { get; set; }
    }
}
