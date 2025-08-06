using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModels
{
    public class AccountDocumentViewModel
    {
        public int AccountDocId { get; set; }
        public int AccountId { get; set; }
        public string Extenstion { get; set; }
        public string DocOrignalName { get; set; }
        public string DocDescription { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public int NewAccountDocId { get; set; }
        public string DocName { get; set; }
    }
}
