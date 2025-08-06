using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class RazorPaySignatureModel
    {
        public string razorpay_payment_id { get; set; }
        public string razorpay_order_id { get; set; }
        public string razorpay_signature { get; set; }
    }
}
