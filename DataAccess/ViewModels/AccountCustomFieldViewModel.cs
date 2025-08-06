using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModels
{
    public class AccountCustomFieldViewModel
    {
        public int AccountCustomFieldId { get; set; }
        public int? AccountCustomFieldValueId { get; set; }
        public int CompanyId { get; set; }
        public string DisplayName { get; set; }
        public string DataType { get; set; }
        public string Control { get; set; }
        public string ControlOption { get; set; }
        public bool IsRequired { get; set; }
        public int AccountId { get; set; }
        public string FieldValue { get; set; }
        public List<string> FieldDataList { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
    }
}
