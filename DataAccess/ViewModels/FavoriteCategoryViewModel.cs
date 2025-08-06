using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModels
{
   public class FavoriteCategoryViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int CategorySelected { get; set; }
    }
}
