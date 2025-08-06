using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModels
{
    public class OrderDetailViewModel
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public int DonerId { get; set; }
        public string ItemName { get; set; }
        public int? Quantity { get; set; }
        public string ItemNumber { get; set; }
        public bool IsSelected { get; set; }
    }
}
