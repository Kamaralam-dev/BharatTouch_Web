using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModels
{
    public class BusinessTypeViewModel
    {
        public int BusinessTypeId { get; set; }
        public string BusinessType { get; set; }
        public int ParentId { get; set; }
        public bool Deleted { get; set; }
        public int DeletedBy { get; set; }
        public int DeletedOn { get; set; }
        public List<KeyValuePair<int, string>> BusinessTypes { get; set; }
    }
}
