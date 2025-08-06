using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModels
{
    public class AuthenticateAppUserViewModel
    {
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyDisplayName { get; set; } 
        public string EmailId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UrlCode { get; set; }
        public string Displayname { get; set; }
        public string UserType { get; set; }
        public bool IsActive { get; set; }
        public bool IsCompanyAdmin { get; set; }
        public bool IsAdmin { get; set; }
        public string AuthToken { get; set; }
    }
}
