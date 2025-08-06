using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class CardTemplateModel
    {
        public int TemplateId { get; set; }
        public string Name { get; set; }
        public string TemplateView { get; set; }
        public string ImageUrl { get; set; }
        public string Types { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
