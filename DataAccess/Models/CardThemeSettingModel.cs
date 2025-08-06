using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class CardThemeSettingModel
    {
        public int ThemeSettingId { get; set; }
        public int TemplateId { get; set; }
        public int UserId { get; set; }
        public string BackgroundImg { get; set; }
    }
}
