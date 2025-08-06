using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class BusinessTypeModel
    {
        public int BusinessTypeId { get; set; }
        public string BusinessType { get; set; }
        public string ParentBusinessType { get; set; }
        public int ParentId { get; set; }
    }
}
