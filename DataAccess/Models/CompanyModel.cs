using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class CompanyModel
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyDisplayName { get; set; }
        public string AdminFirstname { get; set; }
        public string AdminLastname { get; set; }
        public string AdminEmail { get; set; }
        public string AdminDisplayName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Facebook { get; set; }
        public string LinkedIn { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string Youtube { get; set; }
        public string Website { get; set; }
        public string Tagline { get; set; }
        public bool HasBackgroundImage { get; set; }
        public bool HasCompanyVideos { get; set; }
        public bool HasCompanyImages { get; set; }
        public string AboutDescription { get; set; }
        public string SkillName1 { get; set; }
        public string SkillName2 { get; set; }
        public string SkillName3 { get; set; }
        public string SkillName4 { get; set; }
        public string SkillName5 { get; set; }
        public string SkillName6 { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int CreatedBy { get; set; }
    }
}
