using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class RnauraClientModel
    {
        public int Client_Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string AboutProject { get; set; }
        public DateTime? Created_On { get; set; }
    }
}
