using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;

namespace DataAccess.ViewModels
{
    public class WizardCompleteDetailsViewModel
    {
        public UserBasicDetailsModel BasicData { get; set; }
        public AddressModel Address { get; set; }
        public CardStyleModel CardStyle { get; set; }
        public CardOptionsModel CardOptions { get; set; }

    }

    public class UserBasicDetailsModel
    {
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public int CardColorId { get; set; }
    }

    public class AddressModel
    {
        public ShippingAddressModel Shipping { get; set; }
        public BillingAddressModel Billing { get; set; }
    }
    
    public class CardStyleModel
    {
        public int CardColorId { get; set; }
        public int CardFinishId { get; set; }
        public bool IncludeMetalCard { get; set; }
        public string CardType { get; set; }
        public decimal Price { get; set; }
    }
    public class CardOptionsModel
    {
        public string NfcCardLine1 { get; set; }
        public string NfcCardLine2 { get; set; }
        public string NfcCardLine3 { get; set; }
    }
    public class ShippingAddressModel
    {
        public bool IsSelfPick { get; set; }
        public string ShippingAddress1 { get; set; }
        public string ShippingAddress2 { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingCountry { get; set; }
        public string ShippingZip { get; set; }
    }

    public class BillingAddressModel
    {
        public string BillingAddress1 { get; set; }
        public string BillingAddress2 { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingCountry { get; set; }
        public string BillingZip { get; set; }
    }
}
