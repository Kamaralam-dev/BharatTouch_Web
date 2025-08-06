using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModels
{
    public class BulkOrderViewModel
    {
        public int OrderRequestId { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string ContactPerson { get; set; }
        public int MinOrder { get; set; }
        public string Message { get; set; }
    }
}
