using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class UserEducationModel
    {
        public int EducationId { get; set; }
        public int UserId { get; set; }
        public string Institute { get; set; }
        public string University { get; set; }
        public string Degree { get; set; }
        public string Marks { get; set; }
        public string Specialization { get; set; }
        public int PassYear { get; set; }
        public int SortOrder { get; set; }
        public string UpdatedOn { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}
