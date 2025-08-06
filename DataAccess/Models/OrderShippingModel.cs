using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class OrderShippingModel
    {
        public int OrderId { get; set; }
        public int ShippingCompanyid { get; set; }
        public string Trackingnumber { get; set; }
        public string UserFirstName { get; set; }
        public string UserEmailId { get; set; }
        public string OrderNo { get; set; }
        public bool IncludeMetalCard { get; set; }
        public DateTime? Shippingdate { get; set; }
        public string OrderDate { get; set; }
        public string CourierCompany { get; set; }
        public string CourierCompanyUrl { get; set; }
        
    }
}
