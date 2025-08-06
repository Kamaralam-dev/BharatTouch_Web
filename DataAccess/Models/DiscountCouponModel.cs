using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class DiscountCouponModel
    {
        public int DiscountCouponId { get; set; }
        public string CouponName { get; set; }
        public string Code { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal PercentageOff { get; set; }
        public decimal AmountOff { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public string CreatedByEmailId { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
