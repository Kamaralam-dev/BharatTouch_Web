using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class UserByAdminModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int NfcCardColorId { get; set; }
        public int NfcCardFinishId { get; set; }
        public string NfcCardLine1 { get; set; }
        public string NfcCardLine2 { get; set; }
        public string NfcCardLine3 { get; set; }
        public string EmailID { get; set; }
        public int paymentmethodid { get; set; }
        public string DisplayName { get; set; }
        public decimal PackageCost { get; set; }
        public bool ApplyReferralDiscount { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsSelfPick { get; set; }
        public int CreatedBy { get; set; }
        public string DiscountCouponIdText { get; set; }
        public decimal ReferralDiscount { get; set; }
        public decimal CouponDiscount { get; set; }
    }
}
