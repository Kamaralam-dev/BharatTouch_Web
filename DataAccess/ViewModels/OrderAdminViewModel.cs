using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModels
{
    public class OrderAdminViewModel
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int PaymentId { get; set; }
        public string Notes { get; set; }
        public string CustomerName { get; set; }
        public string EmailId { get; set; }
        public string Phone { get; set; }
        public string OrderNo { get; set; }
        public string OrderStatus { get; set; }
        public string TotalAmount { get; set; }
        public string RazorpayOrderId { get; set; }
        public string Tax { get; set; }
        public string PackageCost { get; set; }
        public string ReferralDiscount { get; set; }
        public string CouponDiscount { get; set; }
        public string ShippingCost { get; set; }
        public string PaymentMethod { get; set; }
        public bool InPrinting { get; set; }
        public bool IsSelfPick { get; set; }
        public bool IsShipped { get; set; }
        public int IsHide { get; set; }
        public string NumberCode { get; set; }
        public DateTime? RazorpayOrderDate { get; set; }
        public DateTime? OrderDate { get; set; }
    }
}
