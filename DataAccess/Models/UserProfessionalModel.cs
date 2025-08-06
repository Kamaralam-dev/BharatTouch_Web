using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class UserProfessionalModel
    {
        public int ProfessionId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string Website { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateName { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string UpdatedOn { get; set; }
        public string Phone { get; set; }
    }
}
