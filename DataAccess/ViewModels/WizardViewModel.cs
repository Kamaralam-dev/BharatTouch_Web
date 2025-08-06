using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModels
{
    public class WizardViewModel
    {
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public int Status { get; set; }
        public bool fromSignup { get; set; }
        public int PackageId { get; set; }
    }
}
