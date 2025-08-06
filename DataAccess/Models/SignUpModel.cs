using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class SignUpModel
    {
        public string UserType { get; set; }
        public string EmailId { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public int CountryId { get; set; }
        public string ReferredByCode { get; set; }
        public string Displayname { get; set; }
    }
}
