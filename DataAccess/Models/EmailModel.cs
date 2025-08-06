using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class EmailModel
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string ToEmail { get; set; }
        public string Body { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public string AttachmentPaths { get; set; }
        public bool IsSuccess { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
