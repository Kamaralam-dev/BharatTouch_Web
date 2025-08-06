using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModels
{
    public class OrderViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string UserType { get; set; }
        public string EmailId { get; set; }
        public Boolean IsActive { get; set; }
        public string PersonalEmail { get; set; }
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
        public decimal PackageCost { get; set; }
        public decimal ReferralDiscount { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Tax { get; set; }
        public decimal DiscountedPackageCost { get; set; }
        public Boolean WantMetalCard { get; set; }
    }
}
