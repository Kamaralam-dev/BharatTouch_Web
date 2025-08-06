using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class OrderModel
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int PackageId { get; set; }
        public string OrderNo { get; set; }
        public DateTime? OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; }
        public int PaymentId { get; set; }
        public string DeliveryType { get; set; }
        public string ShippingAddress1 { get; set; }
        public string ShippingAddress2 { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingCountry { get; set; }
        public string ShippingZip { get; set; }
        public string BillingAddress1 { get; set; }
        public string BillingAddress2 { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingCountry { get; set; }
        public string BillingZip { get; set; }
        public string TrackingNumber { get; set; }
        public string CourrierCompany { get; set; }
        public string CourierCompanyURL { get; set; }
        public string Notes { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public Decimal PackageCost { get; set; }
        public decimal Tax { get; set; }
        public string RazorpayOrderId { get; set; }
        public DateTime? RazorpayOrderDate { get; set; }
        public bool IsSelfPick { get; set; }
    }
}
