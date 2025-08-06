using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class UserCertificationModel
    {
        public int CertificationId { get; set; }
        public int UserId { get; set; }
        public string CertificationName { get; set; }
        public string IssuingOrganization { get; set; }
        public string OrganizationURL { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string CertifcateNumber { get; set; }
        public string CertificateURL { get; set; }
        public string Description { get; set; }
        public string CertificateFile { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
