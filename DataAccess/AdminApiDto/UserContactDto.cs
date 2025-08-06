using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.AdminApiDto
{
    public class UserContactDto
    {
        public int UserId { get; set; }
        public string PersonalEmail { get; set; }
        public int CountryId { get; set; }
        public string Phone { get; set; }
        public int? WhatsAppCountryId { get; set; }
        public string Whatsapp { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateName { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }
        public int? WorkPhoneCountryId { get; set; }
        public string WorkPhone { get; set; }
        public int? OtherPhoneCountryId { get; set; }
        public string OtherPhone { get; set; }
        public string OfficeAddress1 { get; set; }
        public string OfficeAddress2 { get; set; }
        public string OfficeCity { get; set; }
        public string OfficeStatename { get; set; }
        public string OfficeCountry { get; set; }
        public string OfficeZip { get; set; }
    }
}
