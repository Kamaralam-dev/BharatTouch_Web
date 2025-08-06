using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class LeadsModel
    {
        public int LeadId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public int LeadTypeId { get; set; }
        public string Message { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
