using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class ClientTestimonialModel
    {
        public int Client_Id { get; set; }
        public string ClientName { get; set; }
        public string Designation { get; set; }
        public string CompanyName { get; set; }
        public int UserId { get; set; }
        public string PicOfClient { get; set; }
        public string PicThumbnailPath { get; set; }
        public string Testimonial { get; set; }
    }
}
