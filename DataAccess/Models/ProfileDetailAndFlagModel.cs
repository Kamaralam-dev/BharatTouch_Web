using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class ProfileDetailAndFlagModel
    {
        //SocialMediaModel
        public int SocialMediaId { get; set; }
        public int UserId { get; set; }
        public string SocialMedia { get; set; }
        public string Url { get; set; }
        public string SortOrder { get; set; }
        public DateTime? UpdatedOn { get; set; }


    }
}
