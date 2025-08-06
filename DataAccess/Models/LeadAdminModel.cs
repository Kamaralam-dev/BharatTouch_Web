using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class LeadAdminModel
    {
        public int LeadId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int CountryId { get; set; }
        public int CreatedBy { get; set; }
        public int CurrentStatusId { get; set; }
        public string Source { get; set; }
        public int AssignedTo { get; set; }
        public string Company { get; set; }
    }
}
