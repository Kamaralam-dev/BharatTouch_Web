using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class ProfileTemplateModel
    {
        public int ProfileTemplateId { get; set; }
        public string TemplateName { get; set; }
        public string TemplateDescription { get; set; }
        public string TemplateFile { get; set; }
        public string TemplateImagePath { get; set; }
        public Boolean IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int CreatedBy { get; set; }
    }
}
