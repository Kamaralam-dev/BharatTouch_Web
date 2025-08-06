using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class InvoiceModel
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }
        public int PackageId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
