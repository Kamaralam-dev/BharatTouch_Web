using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModels
{
    public class JwtTokenViewModel
    {
        public int UserId { get; set; }
        public string EmailId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public bool IsAdmin { get; set; }
    }
}
