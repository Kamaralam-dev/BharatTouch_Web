using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class BlogModel
    {
        public int BlogId { get; set; }
        public string BlogTitle { get; set; }
        public string BlogCategory { get; set; }
        public string BlogUrl { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
