using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class YouTubeModel
    {
        public int YouTubeId { get; set; }
        public string YouTubeTitle { get; set; }        
        public string YouTubeUrl { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
