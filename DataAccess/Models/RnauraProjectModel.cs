using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class RnauraProjectModel
    {
        public int Id { get; set; }
        public string ProjectTitle { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public Boolean IsActive { get; set; }
        public int IsParent { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }

    public class RnauraProjectCategoryModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public Boolean IsActive { get; set; }
    }
}
