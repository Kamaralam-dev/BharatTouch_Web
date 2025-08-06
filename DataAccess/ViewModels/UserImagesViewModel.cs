using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;

namespace DataAccess.ViewModels
{
    public class UserImagesViewModel
    {
        public UserModel user { get; set; }
        public List<TeamViewModel> teams { get; set; }
        public List<ClientTestimonialModel> clientTestimonials { get; set; }
        public UpiDetailsModel upiDetail { get; set; }
    }
}
