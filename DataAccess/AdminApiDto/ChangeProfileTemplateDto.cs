using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.AdminApiDto
{
    public class ChangeProfileTemplateDto
    {
        public int UserId { get; set; }
        public int ProfileTemplateId { get; set; }
    }
}
