using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class PaginationModel
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public string SearchText { get; set; }
        public int? UserId { get; set; }
    }
}
