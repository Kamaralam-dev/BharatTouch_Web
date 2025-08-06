using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class NewsLetterModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
