using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModels
{
    public class LeadsAdminViewModel
    {
        public int LeadId { get; set; }
        public int OrderId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string NumberCode { get; set; }
        public string Phone { get; set; }
        public int CountryId { get; set; }
        public string Country { get; set; }
        public string Source { get; set; }
        public string CreatedByName { get; set; }
        public string CurrentStatus { get; set; }
        public int CreatedBy { get; set; }
        public int CurrentStatusId { get; set; }
        public int AssignedTo { get; set; }
        public string AssignedToName { get; set; }
        public string Company { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
