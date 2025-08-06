using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class OAuthTokenModel
    {
        public int TokenId { get; set; }
        public int UserId { get; set; }
        public string GoogleAccessToken { get; set; }
        public string GoogleRefreshToken { get; set; }
        public string MicrosoftAccessToken { get; set; }
        public string MicrosoftRefreshToken { get; set; }
    }
}
