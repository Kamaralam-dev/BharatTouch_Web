using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModels
{
    public class TeamViewModel
    {
        public int TeamId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string PicturePath { get; set; }
        public string PictureThumbnailPath { get; set; }
    }
}
