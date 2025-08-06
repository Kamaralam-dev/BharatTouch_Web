using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModels
{
    public class NfcCardColorViewModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public int CountryId { get; set; }
        public string CountryCode { get; set; }
        public string NumberCode { get; set; }
        public string DisplayName { get; set; }
        public string EmailId { get; set; }
        public int CardColorId { get; set; }
        public string Color { get; set; }
        public string BackgroundColor { get; set; }
        public string TextColor { get; set; }
        public string Finish { get; set; }
        public string ImagePath { get; set; }
        public string NfcCardLine1 { get; set; }
        public string NfcCardLine2 { get; set; }
        public string NfcCardLine3 { get; set; }
        public int OrderId { get; set; }
        public string OrderNo { get; set; }
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
        public int CardFinishId { get; set; }
        public bool IncludeMetalCard { get; set; }
        public bool IsSelfPick { get; set; }
        public DateTime? CreatedOn { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal PackageCost { get; set; }
        public string CardType { get; set; }
    }
}
