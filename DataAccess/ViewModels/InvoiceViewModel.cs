using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;

namespace DataAccess.ViewModels
{
    public class InvoiceViewModel : InvoiceModel
    {
        public string OrderNo { get; set; }
        public string BillingAddress1 { get; set; }
        public string BillingAddress2 { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingCountry { get; set; }
        public string BillingZip { get; set; }
        public string FullName { get; set; }
        public string EmailId { get; set; }
        public bool WantMetalCard { get; set; }
        public bool IsValidReferral { get; set; }
        public string PackageName { get; set; }
        public decimal PackageCost { get; set; }
        public int PaymentId { get; set; }
        public decimal PaymentAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string TransactionId { get; set; }
        public string RazorPayPaymentMethod { get; set; }
        public string DiscountCoupon { get; set; }
        public decimal ShippingAmount { get; set; }
        public bool IsByHandPickup { get; set; }
        public int DiscountCouponId { get; set; }
        public decimal PercentageOff { get; set; }
        public decimal AmountOff { get; set; }
    }
}
