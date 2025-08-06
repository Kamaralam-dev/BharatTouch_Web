using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class PaymentModel
    {
        public int PaymentId { get; set; }
        public int PaymentMethodId { get; set; }
        public int UserId { get; set; }
        public int PackageId { get; set; }
        public Decimal PaymentAmount { get; set; }
        public Decimal ShippingAmount { get; set; }
        public string DiscountCoupon { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string TransactionId { get; set; }
        public string PaymentStatus { get; set; }
        public int OrderId { get; set; }
        public string Currency { get; set; }
        public string Notes { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string RazorPayPaymentMethod { get; set; }
        public bool IsByHandPickup { get; set; }
        public decimal Tax { get; set; }
        public string RazorPayTransactionId { get; set; }
        public string RazorPayPaymentStatus { get; set; }
        public string RazorPayJson { get; set; }
        public decimal CouponDiscount { get; set; }
        public string GstNo { get; set; }
    }
}
